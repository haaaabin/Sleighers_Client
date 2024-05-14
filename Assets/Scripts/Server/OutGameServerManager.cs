using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OutGameServerManager : MonoBehaviour
{
#region PrivateVariables
    private string serverIP = string.Empty;
    private int serverPort = 0;
    private SocketIOUnity socket;

    [SerializeField]
    private TMP_InputField idInputField;

#endregion

#region PublicVariables
    public static OutGameServerManager instance = null;
#endregion

#region PrivateMethod
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    void Start()
    {
        Init();
        GameManager.MatchMaking += MatchMaking;
    }

    private void Init()
    {
        // serverIP = "localhost"; // ๋ก์ปฌ ??ค?ธ ?ฉ
        serverIP = SecretLoader.outgameServer.ip;
        serverPort = SecretLoader.outgameServer.port;
        socket = new SocketIOUnity("http://" + serverIP +":"+serverPort);

        socket.OnConnected += (sender, e) =>
        {
            Debug.LogFormat("[OutGameServerManager] ?๋ฒ? ? ? ?ฑ๊ณ? {0}:{1}", serverIP, serverPort);
        };

        // ?ฐ๊ฒ? ?ด?  ?ด๋ฒคํธ ?ธ?ค?ฌ
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.LogFormat("[OutGameServerManager] ?๋ฒ? ? ? ?ด?  {0}:{1}", serverIP, serverPort);
        };

        // ??ฌ ?ด๋ฒคํธ ?ธ?ค?ฌ
        socket.OnError += (sender, e) =>
        {
            Debug.LogError("[OutGameServerManager] ??ฌ : " + e);
        };

        // ๋ก๊ทธ?ธ ??ต ?ด๋ฒคํธ ?ธ?ค?ฌ
        socket.On("loginSucc", (res) =>
        {
            Debug.Log("Login success: " + res);
            //SceneManager.LoadScene("Topdown");
            DefaultLoginSucc();
        });

        socket.On("loginFail", (res) =>
        {
            Debug.Log("Login fail: " + res);
        });

        // ??๊ฐ?? ??ต ?ด๋ฒคํธ ?ธ?ค?ฌ
        socket.On("signupSucc", (res) =>
        {
            Debug.Log("Signup success: " + res);
        });

        socket.On("signupFail", (res) =>
        {
            Debug.Log("Signup fail: " + res);
        });

        socket.On("inquiryPlayer", (res) =>
        {
            Debug.Log("inquiryPlayer: " + res);
            string jsonString = res.GetValue<string>();
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonString);
            UserData.instance.id = userInfo.id;
            UserData.instance.nickName = userInfo.name;
            UserData.instance.cart = userInfo.cart;
            UserData.instance.email = userInfo.email;
            Debug.Log("inquiryPlayer: " + userInfo.name);
            Debug.Log("inquiryPlayer: " + userInfo.cart);
            Debug.Log("inquiryPlayer: " + userInfo.email);
        });

        socket.On("setNameSucc", (res) =>
        {
            Debug.Log(res);
        });

        socket.On("setNameFail", (res) =>
        {
            Debug.Log(res);
        });


        socket.On("enterRoomFail", (res) =>
        {
            Debug.Log("enterRoomFail: " + res);
        });

        socket.On("enterRoomSucc", (res) =>
        {
            Debug.Log("endterRoomSucc:" + res);
        });

        socket.On("moveInGameScene", (res) =>
        {
            Debug.Log(res);
        });


        // ?๋ฒ? ?ค?
        socket.Connect();

    }
#endregion

#region PublicMethod
    public static OutGameServerManager Instance()
    {
        if (instance == null)
        {
            Debug.LogError("[OutGameServerManager] ?ธ?ค?ด?ค๊ฐ? ์กด์ฌ?์ง? ??ต??ค.");
            return null;
        }

        return instance;
    }

    public void LoginSucc(string email)
    {
        LoginInfo sendPacket = new LoginInfo();
        sendPacket.email = email;
        Debug.Log("๋ณด๋ธ?ค."+sendPacket);
        string jsonData = JsonUtility.ToJson(sendPacket);
        socket.Emit("loginSucc", jsonData);
    }

    public void DefaultLogin()
    {
        LoginSucc(idInputField.text);
    }

    public void DefaultLoginSucc()
    {
        Debug.Log("DefaultLoginSucc Start");
        OutGameUI.instance.panels[0].SetActive(false);  // auth panel
        OutGameUI.instance.panels[1].SetActive(true);   // lobby panel
        OutGameUI.instance.topBar.SetActive(true);
        Debug.Log("DefaultLoginSucc");
    }

    public void MatchMaking()
    {
        Packet sendPacket = new Packet();
        sendPacket.id = UserData.instance.id;
        Debug.Log("matchmaking id ๋ณด๋ธ?ค."+sendPacket.id);
        string jsonData = JsonUtility.ToJson(sendPacket);
        socket.Emit("matching", jsonData);
        OutGameUI.instance.MatchMakingUI();
    }

    public void SetName()
    {
        SetNameInfo sendPacket = new SetNameInfo();
        sendPacket.id = UserData.instance.id;
        sendPacket.name = OutGameUI.instance.settingNameField.text;
        OutGameUI.instance.settingNameField.text = "";
        Debug.Log("setName ๋ณด๋ธ?ค." + sendPacket);
        string jsonData = JsonUtility.ToJson(sendPacket);
        socket.Emit("setName", jsonData);
    }

#endregion
}