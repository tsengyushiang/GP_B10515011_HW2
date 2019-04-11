using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;  //StreamWrite會用到
using UnityEngine;

public class CustomNetworkGUI : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        GameManager.Instance.RpcReStart();
    }

    public void Start() {

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] configFile = directoryInfo.GetFiles("config.txt");
        printNetwork();

        StreamReader strReader = configFile[0].OpenText();
        GetComponent<NetworkManager>().networkPort = int.Parse(strReader.ReadLine());
        GetComponent<NetworkManager>().networkAddress = strReader.ReadLine().Replace('\\', '/');
    }
 

    public void printNetwork() {

        Debug.Log(GetComponent<NetworkManager>().networkPort);
        Debug.Log(GetComponent<NetworkManager>().networkAddress);

    }

    public void createhost() {

        GetComponent<NetworkManager>().StartHost();
    }

    public void StartClinet() {

        GetComponent<NetworkManager>().StartClient();
    }

    public void reLoadGame() {
        SceneManager.LoadScene(0);
    }

    public void EXIT() {
        Application.Quit();
    }
   
}
