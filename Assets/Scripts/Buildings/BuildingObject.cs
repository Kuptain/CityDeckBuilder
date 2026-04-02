using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InteractionManager;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    [Header("basics")]
    [ReadOnly] Tile tile;
    [ReadOnly] public BuildingData data;
    [ReadOnly] [SerializeField] int rank;
    [Header("construction")]
    [ReadOnly] [SerializeField] bool constructed;
    [ReadOnly] [SerializeField] List<ResourceCost> constructionCost;
    [ReadOnly] [SerializeField] List<ResourceCost> CostStillOpen = new List<ResourceCost>();
    [Header("ability")]
    [ReadOnly] [SerializeField] bool startedToPayForAbility;
    [ReadOnly] [SerializeField] bool hasCD;
    [ReadOnly] [SerializeField] int cooldown;
    [ReadOnly] [SerializeField] int cooldownDuration;
    [ReadOnly] public List<Card> stockedCards;
    [ReadOnly] public HousingValue housingValue;

    MeshRenderer[] outlineRenderers;
    private Material[][] originalMaterials;
    BuildingOutlineStates currentOutlineState;
    public UnityEvent<List<ResourceCost>, List<ResourceCost>> OnConstructionProgress = new UnityEvent<List<ResourceCost>, List<ResourceCost>>();

    private void Start()
    {
        OnConstructionProgress.Invoke(constructionCost, CostStillOpen);
    }

    public void BuildingSetup(BuildingData _data, Tile _tile)
    {
        //references
        data = _data;
        hasCD = TryGetCooldOwnDuration(out cooldownDuration);
        tile = _tile;
        constructionCost = _data.GetBaseCost();
        CostStillOpen = new List<ResourceCost>(constructionCost);
        //events
        TurnManager.OnEndTurn.AddListener(EndOfTurn);
        //BuildEffect();
        //visuals
        outlineRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[outlineRenderers.Length][];

        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            originalMaterials[i] = outlineRenderers[i].materials;
        }

    }
    #region effects

    void BuildEffect()
    {
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onBuild, out effect))
        {
            effect.OnTrigger.Invoke(this, null);
          
        }
    }


    void EndOfTurn()
    {
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onEndOfTurn, out effect))
        {
            effect.OnTrigger.Invoke(this, null);
        }
    }
    public void PlayCardOnThis(Card card)
    {
        if (!constructed)
        {
            PayForConstruction(card);
        }
        if (startedToPayForAbility)
        {

        }
        else
        {
            OnCardEffect(card);
        }
    }
    private void OnCardEffect(Card card)
    {
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onCard, out effect))
        {
            if (card.data.TryToPayFor(effect.EffectCost))
            {
                effect.OnTrigger.Invoke(this, card);
                CardManager.instance.DiscardCard(card, true);
                TurnManager.OnEndTurn.Invoke();
            }
            else
            {
                CardManager.instance.SendLog("Card had not the right ressources");
            }
        }
        else
        {
            CardManager.instance.SendLog("Building had no effect on Card played");
        }
    }
    

    void PayForConstruction(Card card)
    {
        if (card.TryToPayFor(ref CostStillOpen))
        {
            OnConstructionProgress.Invoke(constructionCost, CostStillOpen);
            if (CostStillOpen.Count == 0)
            {

                Constructionfinished();
            }
            CardManager.instance.DiscardCard(card, true);
            TurnManager.OnEndTurn.Invoke();
        }

    }
    public List<ResourceCost> GetCostsStillOpen()
    {
        return CostStillOpen;
    }
    void Constructionfinished()
    {
        BuildingManager.Instance.SendLog(data.name + " constructed");
        constructed = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        BuildEffect();
    }

    public void FinishConstruction()
    {
        constructed = true;
        BuildEffect();
    }
    public void AddCardToStock(Card card)
    {
        stockedCards.Add(card);
    }
    public void TryToCraft()
    {

    }
    #endregion
    public void VisualProductionCards()
    {
        Vector3 pos = GridManager.Instance.GridToWorldPosition(tile.gridPosition);
        for (int i = 0; i < GetCurrentCards().Count; i++)
        {
            IconAnimationManager.instance.Anim_ResourceToProductionDeck(GetCurrentCards()[i], pos);
        }
    }


    #region utility
    public Tile GetTile()
    {
        return tile;
    }
    public int GetRank()
    {
        return rank;
    }
    bool TryGetCooldOwnDuration(out int duration)
    {
        duration = 0;
        List<BuildingEffect> effects = data.GetRankData(rank).effects;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].HasCoolDown)
            {
                duration = effects[i].cooldownDuration;
                return true;
            }
        }
        return false;
    }
    public List<Card_Data> GetCurrentCards()
    {
        return data.GetRankData(rank).cardsToAdd;
    }
    public bool TryToGetBuildingEffect(BuildingEffect.triggerType type, out BuildingEffect effect)
    {
        List<BuildingEffect> effects = data.GetRankData(rank).effects;
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].type == type)
            {
                effect = effects[i];
                return true;
            }
        }
        effect = null;
        return false;
    }
    #endregion

    #region feedback
    public void Click()
    {
        //open context Tooltip of the building
    }
    public void StartHover(BuildingOutlineStates state)
    {
        EnableOutline(state);
    }
    public void StopHover()
    {
        EnableOriginMaterials();
    }
    public void EnableOutline(BuildingOutlineStates state)
    {
        if (currentOutlineState == state) return;
        if (currentOutlineState != BuildingOutlineStates.Idle)
        {
            EnableOriginMaterials();
        }
        currentOutlineState = state;

        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            Material[] mats = outlineRenderers[i].materials;

            Material[] newMats = new Material[mats.Length + 1];

            for (int j = 0; j < mats.Length; j++)
            {
                newMats[j] = mats[j];
            }
            if (state == BuildingOutlineStates.Hover)
            {
                newMats[mats.Length] = BuildingManager.Instance.outlineHover;

            }
            else if (state == BuildingOutlineStates.Draggable)
            {
                newMats[mats.Length] = BuildingManager.Instance.outlineDragCard;

            }

            outlineRenderers[i].materials = newMats;
        }
    }
    public void EnablePreviewMaterials()
    {
        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            outlineRenderers[i].materials = originalMaterials[i];
        }
        currentOutlineState = BuildingOutlineStates.Idle;
    }
    public void EnableOriginMaterials()
    {
        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            outlineRenderers[i].materials = originalMaterials[i];
        }
        currentOutlineState = BuildingOutlineStates.Idle;
    }
    #endregion
}
