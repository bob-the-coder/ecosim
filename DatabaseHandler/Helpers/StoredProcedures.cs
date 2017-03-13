namespace DatabaseHandler.Helpers
{
	public enum StoredProcedures
	{
        ClearDatabase,

        SimSettingsCreate,
        SimSettingsGet,

        ProductCreate,
        ProductUpdate,
        ProductRemove,

        ProductGet,
        ProductGetList,
        ProductGetAll,

        ProductIngredientAssign,

        ProductionCreate,
        ProductionUpdate,
        ProductionDelete,
        ProductionGetAll,

        NodeCreate,
        NodeUpdate,
        NodeDelete,
        NodeGet,
        NodeGetList,
        NodeGetAll,

        NodeLinkCreate,
        NodeLinkDelete,
        NodeLinkGetAll,

        NeedCreate,
        NeedUpdate,
        NeedDelete,
        NeedGetAll,

        LinkGetAllVisJs,
        NodeGetAllVisJs,

        FullSimulation_GetNodes,
        FullSimulation_GetProducts,
        FullSimulation_GetLinks,
        FullSimulation_GetProductions,
        FullSimulation_GetNeeds,
        FullSimulation_GetSimulation,
        FullSimulation_GetSimulationLogs,
        FullSimulation_GetDecisionChances,

        DecisionChanceCreate,
        SessionLogCreate
    }
}
