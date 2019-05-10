exec fz_getSAEDIClient @SAEDIID=N'3CCH23DE',@ProgramVersion=N'2'

exec fz_getSAEDIAddressesByClientID @Client_id=N'3CCH23DE'

exec fz_GetClientProviderSettings @SaediId=N'3CCH23DE'

exec fz_getTaxRate @TaxId='STD'

exec fz_verifySaedi @id=N'JTM498',@name=N'JTM',@password=N'{C8FACF63-5608-4FC1-90E3-78F62499BF26}',@lastTransfer='2019-05-10 11:27:32'

exec fz_getVulcanInspectionList @fromId='JTM498'

SELECT TOP 100 * FROM [InventoryAppliance] WHERE [toId] = 'JTM498' AND (Sent is null)

exec fz_getVulcanServiceNotes @siteId='JTM498'

SELECT TOP 100 * FROM [SAEDIMedia] WHERE ([SaediToId] = 'JTM498') AND ([LastTransfer] IS NULL)

exec JobCancelExpiredCalls 

exec fz_getSAEDIClient @SAEDIID=N'3CCH23DE',@ProgramVersion=N'2'

exec fz_getSAEDIAddressesByClientID @Client_id=N'3CCH23DE'

exec fz_GetClientProviderSettings @SaediId=N'3CCH23DE'

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'

exec fz_AddUsage @SaediId=N'3CCH23DE',@type=N'Desktop',@info=N'chrome 7.0'

exec fz_SAEDICalls_GetModelsByDateCounts @saediId='3CCH23DE',@thedate='2019-05-10'

exec fz_ServiceNotesgetBySAEDIToIDCount @SAEDIToID='3CCH23DE'

exec fz_SAEDICalls_GetModelsAllClosed_Count @saediId=N'3CCH23DE'

exec fz_SAEDICalls_GetModelsRMACollection_Count @saediId=N'3CCH23DE'

exec fz_SAEDICalls_GetModelsS2C_Count @saediId=N'3CCH23DE'

exec fz_SAEDICalls_GetModelsAllWIPCounts @saediId='3CCH23DE'

exec fz_SAEDICalls_GetModelsNewCallsCounts @saediId='3CCH23DE'

exec fz_SAEDICalls_GetModelsNoDateCounts @saediId='3CCH23DE'

exec fz_SAEDICalls_GetModelsIncompleteCounts @saediId='3CCH23DE'

exec fz_SAEDICalls_GetModelsRequiresOrderCounts @saediId='3CCH23DE'

exec fz_SAEDICalls_GetCallsByStatusForEngineer @SaediId=N'3CCH23DE'

exec fz_getCompaniesForEngineer @SaediToID=N'3CCH23DE'

exec fz_GetNewsBySaediID @SAEDIContactSAEDIID='ADMINFIX'

exec fz_GetNewsBySaediID @SAEDIContactSAEDIID='SONY3C'

exec fz_GetNewsBySaediID @SAEDIContactSAEDIID='SONYCC'

exec fz_GetNewsBySaediID @SAEDIContactSAEDIID='3CCH23DE'

exec fz_GetContactsBySaediID @SAEDIContactSAEDIID='ADMINFIX'

exec fz_GetContactsBySaediID @SAEDIContactSAEDIID='SONY3C'

exec fz_GetContactsBySaediID @SAEDIContactSAEDIID='SONYCC'

exec fz_GetContactsBySaediID @SAEDIContactSAEDIID='3CCH23DE'

exec fz_SAEDICalls_GetModelsNewCalls_Paged @saediId='3CCH23DE',@startRowIndex=0,@maxNumRows=1000

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'

exec fz_getOSPURLByClientID @Client_id=N'3CCH23DE'