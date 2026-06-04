using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static InteractionManager;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    [Header("basics")]
    public Transform buildingVisualTransform;
    [ReadOnly] Tile tile;
    [ReadOnly] public BuildingData data;
    [ReadOnly] [SerializeField] int rank;
    [ReadOnly] public bool isConstructing;
    [Header("construction")]
    [ReadOnly] public EffectCostVisualizer constructionUI;
    [ReadOnly] public bool isLocked;
    [Header("ability")]
    [ReadOnly] [SerializeField] OpenEffect openEffect;
    [ReadOnly] [SerializeField] bool hasCD;
    [ReadOnly] [SerializeField] int cooldown;
    [ReadOnly] [SerializeField] int cooldownDuration;
    [ReadOnly] public List<ResourceCard> stockedCards;
    [ReadOnly] public HousingValue housingValue;

    MeshRenderer[] outlineRenderers;
    private Material[][] originalMaterials;
    BuildingOutlineStates currentOutlineState;
    public UnityEvent<OpenEffect> OnEffectProgress = new UnityEvent<OpenEffect>();

    private void Start()
    {

    }
    public void BuildingPreviewSetup(BuildingData _data)
    {
        //references
        data = _data;

        //visuals
        outlineRenderers = buildingVisualTransform.GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[outlineRenderers.Length][];

        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            originalMaterials[i] = outlineRenderers[i].materials;
        }
        EnablePreviewMaterials();
    }
    public void BuildingSetup(BuildingData _data, Tile _tile, bool _isLocked)
    {
        //references
        data = _data;
        hasCD = TryGetCooldOwnDuration(out cooldownDuration);
        cooldown = cooldownDuration;
        tile = _tile;
        isLocked = _isLocked;
        if (isLocked)
        {
            var unlockCost = new List<ResourceCost>();
            var resource = BuildingManager.Instance.blueprintCost;
            unlockCost.Add(resource);
            openEffect = new OpenEffect(OpenEffect.Type.construction, unlockCost, BuildingUnlocked);
        }
        else
        {
            openEffect = new OpenEffect(OpenEffect.Type.construction, _data.GetBaseCost(), ConstructionFinished);
        }
        isConstructing = true;
        constructionUI = Instantiate(BuildingManager.Instance.buildingConstructionUIPrefab, transform.position, Quaternion.identity, transform).GetComponent<EffectCostVisualizer>();
        constructionUI.Init(this);
        constructionUI.ToggleVisible(tile.isExplored);
        OnEffectProgress.Invoke(openEffect);
        //events
        TurnManager.OnEndTurn.AddListener(OnEndOfTurnEffect);
    }
    #region effects

    public void StartUpgrade()
    {
        if (openEffect == null || !openEffect.active)
        {
            openEffect = new OpenEffect(OpenEffect.Type.upgrade, data.GetRankData(rank + 1).resourceCosts, IncreaseRank);
            OnEffectProgress.Invoke(openEffect);
            isConstructing = true;
        }
    }

    void IncreaseRank()
    {
        rank += 1;
        OnBuildEffect();
        isConstructing = false;
    }

    void OnBuildEffect()
    {
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onBuild, out effect))
        {
            effect.Invoke(this, null);

        }
    }


    void OnEndOfTurnEffect()
    {
        Debug.Log("end of turn");
        cooldown += 1;
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onEndOfTurn, out effect))
        {
            effect.Invoke(this, null);
        }
    }
    public void PlayCardOnThis(ICard card)
    {
        if (card.GetType() == CardType.Resource)
        {
            if (openEffect != null && openEffect.CostsStillOpen.Count > 0)
            {
                PayForOpenEffect((ResourceCard)card);
            }
            else
            {
                OnCardEffect((ResourceCard)card);
            }
        }
        else
        {
            //respond to character beeing played on this
        }
    }
    private void OnCardEffect(ResourceCard card)
    {
        
        BuildingEffect effect;
        if (TryToGetBuildingEffect(BuildingEffect.triggerType.onCard, out effect))
        {
            if ( !IsOnCooldown() && card.data.TryToPayFor(effect.EffectCost))
            {
                effect.Invoke(this, card);
                //CardManager.instance.DiscardCard(card, true);
                cooldown = 0;
                //TurnManager.OnEndTurn.Invoke();

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


    void PayForOpenEffect(ResourceCard card)
    {
        
        if (openEffect.Contains(card.GetCurrentResources()))
        {
            openEffect.PayCosts(card.GetCurrentResources());
            OnEffectProgress.Invoke(openEffect);
            if (openEffect.CostsStillOpen.Count == 0)
            {
                openEffect = null;
            }
            //CardManager.instance.DiscardCard(card, true);
        }
        
    }

    public void CancleOpenEffect()
    {
        if (openEffect!=null && openEffect.active)
        {
            if (openEffect.type == OpenEffect.Type.construction)
            {
                BuildingManager.Instance.DestroyBuilding(tile.gridPosition);
                Destroy(gameObject);
            }
            openEffect = null;
            OnEffectProgress.Invoke(openEffect);
        }

    }

    public List<ResourceCost> GetCostsStillOpen()
    {
        if (openEffect == null)
        {
            return new List<ResourceCost>();
        }
        return openEffect.CostsStillOpen;
    }

    void BuildingUnlocked()
    {
        BuildingManager.Instance.UnlockBuilding(data);
        BuildingManager.Instance.DestroyBuilding(tile.gridPosition);
        Destroy(gameObject);
    }

    public void ConstructionFinished()
    {
        isConstructing = false;
        OnBuildEffect();
    }
    public void ForceConstructionFinished()
    {
        isConstructing = false;
        OnBuildEffect();
        openEffect = null;
    }
    public void AddCardToStock(ResourceCard card)
    {
        stockedCards.Add(card);
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

    #region crafting


    public bool TryToGetCraftingRecipes(out List<CraftRecipe> recipes)
    {
        recipes = data.GetRankData(rank).craftingRecipes;
        return data.GetRankData(rank).usesCrafting;
    }



    CraftRecipe currentActiveRecipe;
    public void Craft(CraftRecipe recipe)
    {
        Debug.Log("try to craft");
        if (openEffect == null || !openEffect.active)
        {
            Debug.Log("start crafting");
            currentActiveRecipe = recipe;
            openEffect = new OpenEffect(OpenEffect.Type.other, recipe.costs, FinishRecipe);
            OnEffectProgress.Invoke(openEffect);
        }
    }
    void FinishRecipe()
    {
        for (int i = 0; i < currentActiveRecipe.cardsToCreate.Count; i++)
        {
            CardManager.instance.GetTemporaryCard(currentActiveRecipe.cardsToCreate[i]);
        }
    }


    #endregion

    #region utility
    public Tile GetTile()
    {
        if(GridManager.Instance.TryGetTile(tile.gridPosition, out Tile newTile))
        {
            tile = newTile;
            return newTile;
        }
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

    public bool HasUpgrade()
    {
        return data.GetRankData(rank + 1) != null;
    }

    public bool HasOpenEffect(out OpenEffect effect)
    {
        effect = openEffect;
        return openEffect != null && openEffect.active;
    }
    public bool HasOpenEffect()
    {
        return openEffect != null && openEffect.active;
    }

    public bool IsOnCooldown()
    {
        return cooldown < cooldownDuration;
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
            var mats = outlineRenderers[i].materials;

            for (int m = 0; m < mats.Length; m++)
            {
                mats[m] = BuildingManager.Instance.matPreviewBuilding;
            }

            outlineRenderers[i].materials = mats;
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

    public void EnableOriginMaterial(int childPos) // BROKEN!!
    {
        Debug.Log("BuildingObject: EnableOriginMaterial, childPos: " + childPos);

        for (int i = 0; i < outlineRenderers.Length; i++)
        {
            if (outlineRenderers[i].transform.parent == buildingVisualTransform.GetChild(childPos))
            {
                var mats = outlineRenderers[i].materials;

                for (int m = 0; m < mats.Length; m++)
                {
                    var currentMat = originalMaterials[i][m];
                    mats[m] = currentMat;
                    Debug.Log("BuildingObject: EnableOriginMaterial, m: " + m);
                }

                outlineRenderers[i].materials = mats;
            }
        }
        currentOutlineState = BuildingOutlineStates.Idle;
    }

    #endregion

    #region debug

    [ContextMenu("Trigger active Effect")]
    void PayConstrcuctionCost()
    {
        if (openEffect != null)
        {
            openEffect.PayCosts(openEffect.CostsStillOpen);
            OnEffectProgress.Invoke(openEffect);
        }
    }

    #endregion
    [System.Serializable]
    public class OpenEffect
    {
        [ReadOnly] [SerializeField] List<ResourceCost> costs = new List<ResourceCost>();
        [ReadOnly] [SerializeField] List<ResourceCost> costsPayed = new List<ResourceCost>();
        [ReadOnly] [SerializeField] List<ResourceCost> openCosts = new List<ResourceCost>();
        bool costsDirty;
        public Type type;
        public List<ResourceCost> Costs { get { return costs; } set { costsDirty = true; costs = value; } }
        public List<ResourceCost> CostsPayed { get { return costsPayed; } set { costsDirty = true; costsPayed = value; } }
        public List<ResourceCost> CostsStillOpen
        {
            get
            {
                if (costsDirty)
                {
                    List<ResourceCost> _openCosts = new List<ResourceCost>();
                    for (int i = 0; i < Costs.Count; i++)
                    {
                        _openCosts.Add(new ResourceCost(Costs[i].resource, Costs[i].amount));
                    }
                    for (int i = _openCosts.Count - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < CostsPayed.Count; j++)
                        {
                            _openCosts[i].Subtract(CostsPayed[j]);
                        }
                        if (_openCosts[i].amount <= 0)
                        {
                            _openCosts.RemoveAt(i);
                        }
                    }
                    openCosts = _openCosts;
                    costsDirty = false;
                    return _openCosts;
                }
                else
                {
                    if(openCosts == null)
                    {
                        openCosts = new List<ResourceCost>();
                        costsDirty = true;
                    }
                    return openCosts;
                }
            }
        }
        public bool active = true;
        public UnityEvent OnFinish = new UnityEvent();

        public OpenEffect(Type effectType, List<ResourceCost> _costs, UnityAction _onFinish)
        {
            OnFinish.AddListener(_onFinish);
            Costs = new List<ResourceCost>();
            for (int i = 0; i < _costs.Count; i++)
            {
                Costs.Add(new ResourceCost(_costs[i].resource, _costs[i].amount));
            }
            type = effectType;
        }
        public void PayCosts(List<ResourceCost> costs)
        {
            CostsPayed.AddRange(costs);
            costsDirty = true;
            if (CostsStillOpen.Count <= 0)
            {
                OnFinish.Invoke();
                active = false;
            }
        }

        public bool Contains(List<ResourceCost> _costs)
        {
            for (int i = 0; i < CostsStillOpen.Count; i++)
            {
                for (int j = 0; j < _costs.Count; j++)
                {
                    if (CostsStillOpen[i].resource == _costs[j].resource)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        public enum Type
        {
            construction = 0,
            upgrade = 1,
            other = 2
        }
    }
}
