//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

using UnityEngine;
using System.Collections;
using Amazon.DynamoDBv2;
using UnityEngine.UI;
using Amazon;
using UnityEngine.SceneManagement;

namespace AWSSDK.Examples
{
    public class DynamoDBExample : MonoBehaviour
    {

        public Button lowLevelButton;
        public Button midLevelScanButton;
        public Button highLevelobjectMapperButton;

        // Use this for initialization
        void Start()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
            lowLevelButton.onClick.AddListener(LowLevelListener);
            midLevelScanButton.onClick.AddListener(MidLevelScanListener);
            highLevelobjectMapperButton.onClick.AddListener(HighLevelListener);

            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }

        void LowLevelListener()
        {
            SceneManager.LoadScene(3);
        }

        void MidLevelScanListener()
        {
            SceneManager.LoadScene(4);
        }

        void HighLevelListener()
        {
            SceneManager.LoadScene(5);
        }
    }
}
