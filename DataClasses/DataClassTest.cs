using System;
using System.Text;
using UnityEngine;

public class DataClassTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var json = @"{""messagetype"":""position"",""id"":""974682a"",""collector_id"":""tac-b"",""collector_serial"":""051001572"",""total_detected_objects"":1,""object_list"":{""object0"":{""id"":76,""speed"":0.0,""x"":[-75,-93,0,0,0,0,0,0,0,0],""y"":[40,51,0,0,0,0,0,0,0,0]}}}";

        Payload? payload = JsonUtility.FromJson<MyClass>(json);
        Debug.Log(payload);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
