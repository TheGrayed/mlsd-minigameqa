using System.Net.Sockets;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;
using System;

public class Input : StandaloneInputModule
{
    //TODO: Overwrite input


    [System.Serializable]
    public class AxisData
    {
        public string axis_name_;
        public float axis_value_;
    }

    [System.Serializable]
    public class ButtonData
    {
        public KeyCode key_;
        public bool down_;
        public bool prev_down_;
    }

    public enum InputEntryType
    {
        Button,
        Axis,
        MousePosition
    }

    [System.Serializable]
    public class SchemaEntry
    {
        public InputEntryType type_;
        public int index_;
        //public bool fire_even_;
    }

    private static Input singleton_;

    [SerializeField]
    private bool ai_possessed_ = false;
    [SerializeField]
    private bool auto_hide_error_console_ = true;

    public string ai_ip_ = "127.0.0.1";
    public int ai_port_ = 20001;
    private IPEndPoint remote_end_point_;
    private UdpClient client_;

    [SerializeField]
    private SchemaEntry[] ai_schema_;
    [SerializeField]
    private AxisData[] ai_axis_data_;
    [SerializeField]
    private ButtonData[] ai_button_data_;
    [SerializeField]
    private Vector2 mouse_position_;

    public static Input Singleton
    {
        get
        {
            if (singleton_ == null)
            {
                singleton_ = FindFirstObjectByType<Input>();
                if (singleton_ == null)
                {
                    var standalone_input = FindObjectOfType<StandaloneInputModule>().gameObject;
                    var pos = standalone_input.transform.position;
                    var rot = standalone_input.transform.rotation;
                    var parent = standalone_input.transform.parent;
                    Destroy(standalone_input);
                    GameObject ai_input = Instantiate(Resources.Load("aiinput", typeof(GameObject)), pos, rot, parent) as GameObject;
                    singleton_ = ai_input.GetComponent<Input>();
                }
            }
            return singleton_;
        }
    }

    public static bool AIPosessed
    {
        get
        {
            return Singleton.ai_possessed_;
        }
    }

    protected override void Awake()
    {
        if (auto_hide_error_console_)
            Debug.developerConsoleEnabled = false;
        
        if (singleton_ != null)
            Debug.LogError("Input already exists");
        singleton_ = this;
        base.Awake();

        string[] arguments = Environment.GetCommandLineArgs();
#if !UNITY_EDITOR
        Debug.Log("Commandline Args:\n" + string.Join('\n',arguments));
#endif
        foreach (string arg in arguments)
        {
            if (arg.ToLower() == "-ai-possessed")
            {
                Debug.Log("Setting ai_possessed_");
                ai_possessed_ = true;
            }
        }

        if (AIPosessed)
        {
            remote_end_point_ = new IPEndPoint(IPAddress.Parse(ai_ip_), ai_port_);
            client_ = new UdpClient();
            client_.Client.ReceiveTimeout = 3000;
            client_.Client.SendTimeout = 3000;
            client_.Connect(remote_end_point_);
        }
    }

    

    public static Vector3 mousePosition
    {
        get
        {
            if (AIPosessed)
            {
                return Singleton.mouse_position_;
            }
            else
            {
                return UnityEngine.Input.mousePosition;
            }
        }
    }

    public static float GetAxisRaw(string name)
    {
        if (AIPosessed)
        {
            for (int i = 0; i < Singleton.ai_axis_data_.Length; ++i)
                if (Singleton.ai_axis_data_[i].axis_name_ == name)
                    return Singleton.ai_axis_data_[i].axis_value_;
        }
        return UnityEngine.Input.GetAxisRaw(name);
    }

    public static bool GetKeyDown(KeyCode key)
    {

        if (AIPosessed)
        {
            for (int i = 0; i < Singleton.ai_button_data_.Length; ++i)
                if (Singleton.ai_button_data_[i].key_ == key)
                    return Singleton.ai_button_data_[i].down_;
        }
        return UnityEngine.Input.GetKeyDown(key);
    }

    private void Update()
    {
        if (AIPosessed)
        {
            string message = Time.frameCount.ToString("D8") + "," + Time.time.ToString();
            byte[] stringList = Encoding.UTF8.GetBytes(message);
            client_.Send(stringList, stringList.Length);
            byte[] datagram = client_.Receive(ref remote_end_point_);
            string[] ai_inputs = Encoding.UTF8.GetString(datagram).Split(',');
            if (ai_inputs.Length != ai_schema_.Length)
                Debug.LogError("Schema and input length mismatch");
            for (int i = 0; i < ai_inputs.Length; ++i)
            {
                switch (ai_schema_[i].type_)
                {
                    case InputEntryType.Button:
                        var button_data = ai_button_data_[ai_schema_[i].index_];
                        button_data.prev_down_ = button_data.down_;
                        button_data.down_ = Convert.ToBoolean(Convert.ToInt16(ai_inputs[i]));
                        //Debug.Log(button_data.prev_down_.ToString() + "->" + button_data.down_.ToString());
                        if (button_data.key_ == KeyCode.Mouse0 && button_data.down_ != button_data.prev_down_)
                        {
                            //Debug.Log(i);
                            //Debug.Log(ai_schema_[i].index_);
                            //Debug.Log(button_data.key_);
                            //Debug.Log((int)ai_button_data_[ai_schema_[i].index_].key_);
                            //Debug.LogError("Calling click at " + mousePosition.ToString());
                            ClickAt(button_data.down_);
                        }
                        break;

                    case InputEntryType.Axis:
                        ai_axis_data_[ai_schema_[i].index_].axis_value_ = Convert.ToSingle(ai_inputs[i]);
                        break;

                    case InputEntryType.MousePosition:
                        var elements = ai_inputs[i].Split('~');
                        mouse_position_ = new Vector2(Convert.ToSingle(elements[0]), Convert.ToSingle(elements[1]));
                        break;

                }
            }


            //Debug.Log();
        }
    }

    private void OnApplicationQuit()
    {
        if (AIPosessed)
            client_.Close();
    }

    private void ClickAt(bool pressed)
    {
        UnityEngine.Input.simulateMouseWithTouches = true;
        var pointerData = GetTouchPointerEventData(new Touch()
        {
            position = mousePosition,
        }, out bool b, out bool bb);

        ProcessTouchPress(pointerData, pressed, !pressed);
    }
}
