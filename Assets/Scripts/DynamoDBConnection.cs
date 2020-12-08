/*
 * Author: Leslie
 * Last Modified by: Leslie
 * Date Last Modified: 2020-11-05
 * Program Description: Amazon Dynamo DB connection setup
 * Revision History:
 *      - Initial Setup
 */

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
