using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Threading;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime.Internal;
using System.Collections.Generic;
using Amazon.Util;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;

public class WikipediaDBPull : DynamoDBBaseConnection
{
    public Text bacteriaButtonText;
    public Text bacteriaDescriptionHeaderText;
    public Text bacteriaDescriptionBodyText;

    public Text virusButtonText;
    public Text virusDescriptionHeaderText;
    public Text virusDescriptionBodyText;

    private IAmazonDynamoDB _client;
    private DynamoDBContext _context;

    private string resultText;

    int bacteriaID = 1;
    int virusID = 2;

    private DynamoDBContext Context
    {
        get
        {
            if (_context == null)
                _context = new DynamoDBContext(_client);

            return _context;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        _client = Client;
    }

    public void GetInfoFromDB()
    {
        // Retrieve the bacteria info. 
        EnemyInfo bacteriaRetrieved = null;
        Context.LoadAsync<EnemyInfo>(bacteriaID, (result) =>
        {
            if (result.Exception == null)
            {
                bacteriaRetrieved = result.Result as EnemyInfo;

                Debug.Log("bacteriaRetrieved.EnemyName: " + bacteriaRetrieved.EnemyName);
                Debug.Log("bacteriaRetrieved.Description: " + bacteriaRetrieved.Description);

                //bacteriaButtonText.text = bacteriaRetrieved.EnemyName;
                //bacteriaDescriptionHeaderText.text = bacteriaRetrieved.EnemyName;
                //bacteriaDescriptionBodyText.text = bacteriaRetrieved.Description;
            }
        });


        // Retrieve the virus info. 
        EnemyInfo virusRetrieved = null;
        Context.LoadAsync<EnemyInfo>(virusID, (result) =>
        {
            if (result.Exception == null)
            {
                virusRetrieved = result.Result as EnemyInfo;

                Debug.Log("virusRetrieved.EnemyName: " + virusRetrieved.EnemyName);
                Debug.Log("virusRetrieved.Description: " + virusRetrieved.Description);

                //virusButtonText.text = virusRetrieved.EnemyName;
                //virusDescriptionHeaderText.text = virusRetrieved.EnemyName;
                //virusDescriptionBodyText.text = virusRetrieved.Description;
            }
        });
    }

    [DynamoDBTable("EnemyInfo")]
    public class EnemyInfo
    {
        [DynamoDBHashKey]   // Hash key.
        public int Id { get; set; }
        [DynamoDBProperty]
        public string EnemyName { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
    }
}
