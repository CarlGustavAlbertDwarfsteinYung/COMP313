using UnityEngine;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TMPro;

public class WikipediaDBPull : DynamoDBBaseConnection
{
    public TextMeshProUGUI bacteriaButtonText;
    public TextMeshProUGUI bacteriaDescriptionHeaderText;
    public TextMeshProUGUI bacteriaDescriptionBodyText;

    public TextMeshProUGUI virusButtonText;
    public TextMeshProUGUI virusDescriptionHeaderText;
    public TextMeshProUGUI virusDescriptionBodyText;

    private IAmazonDynamoDB _client;
    private DynamoDBContext _context;

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

    void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        _client = Client;
    }

    /// <summary>
    /// Pulls the information from the Amazon DynamoDB
    /// </summary>
    public void GetInfoFromDB()
    {
        // Retrieve the bacteria info. 
        EnemyInfo bacteriaRetrieved = null;

        Context.LoadAsync<EnemyInfo>(bacteriaID, (result) =>
        {
            if (result.Exception == null)
            {
                bacteriaRetrieved = result.Result as EnemyInfo;

                bacteriaButtonText.text = bacteriaRetrieved.EnemyName;
                bacteriaDescriptionHeaderText.text = bacteriaRetrieved.EnemyName;
                bacteriaDescriptionBodyText.text = bacteriaRetrieved.Description;
                bacteriaDescriptionBodyText.overflowMode = TextOverflowModes.Page;
            }
        });


        // Retrieve the virus info. 
        EnemyInfo virusRetrieved = null;
        Context.LoadAsync<EnemyInfo>(virusID, (result) =>
        {
            if (result.Exception == null)
            {
                virusRetrieved = result.Result as EnemyInfo;

                virusButtonText.text = virusRetrieved.EnemyName;
                virusDescriptionHeaderText.text = virusRetrieved.EnemyName;
                virusDescriptionBodyText.text = virusRetrieved.Description;
                virusDescriptionBodyText.overflowMode = TextOverflowModes.Page;
            }
        });
    }

    [DynamoDBTable("EnemyInfo")]
    public class EnemyInfo
    {
        [DynamoDBHashKey]   // Hash key
        public int Id { get; set; }
        [DynamoDBProperty]
        public string EnemyName { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
    }
}
