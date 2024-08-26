using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SheetReader
{
    //Credentials and spreadsheet ID
    static private String spreadsheetId = "1v0P0dl_ClKxOELuHHl0_1FmbjfI4wAITmIv1BZa6NZg";
    static private String jsonPath = "/StreamingAssets/key.json";

    static private SheetsService service;

    //Reads the spreadsheet and returns a list of lists of strings
    static SheetReader()
    {
        String fullJsonPath = Application.dataPath + jsonPath;

        Stream jsonCreds = (Stream)File.Open(fullJsonPath, FileMode.Open);

        ServiceAccountCredential credential = ServiceAccountCredential.FromServiceAccountData(jsonCreds);

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });
    }


    //Searches for a range of cells in a spreadsheet
    public IList<IList<object>> getSheetRange(String sheetNameAndRange)
    {
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            return values;
        }
        else
        {
            Debug.Log("No data found.");
            return null;
        }
    }
}
