using UnityEngine;
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
