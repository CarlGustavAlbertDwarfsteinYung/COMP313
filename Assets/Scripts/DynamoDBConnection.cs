using UnityEngine;
using System.Collections;
using Amazon.DynamoDBv2;
using UnityEngine.UI;
using Amazon;

public class DynamoDBConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
