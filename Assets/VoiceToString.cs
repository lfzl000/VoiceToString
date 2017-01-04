﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;

public class VoiceToString : MonoBehaviour
{
    private string token;                           //access_token
    private string cuid = "BIMSOPzhaoliang0710";        //用户标识
    private string format = "wav";                  //语音格式
    private int rate = 8000;                        //采样率
    private int channel = 1;                        //声道数
    private string speech;                          //语音数据，进行base64编码
    private int len;                                //原始语音长度
    private string lan = "zh";                      //语种

    private string grant_Type = "client_credentials";  
    private string client_ID = "3Dpp3SLlEGn4CLSgH65UVjkr";                       //百度appkey
    private string client_Secret = "4f7869a6ae3da67db1c689883357858e";                   //百度Secret Key

    private string baiduAPI = "http://vop.baidu.com/server_api";
    private string getTokenAPIPath = "https://openapi.baidu.com/oauth/2.0/token";

    private Byte[] clipByte;

    /// <summary>
    /// 转换出来的TEXT
    /// </summary>
    public static string audioToString;

    private AudioSource aud;
    private int audioLength;//录音的长度

    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartMic()
    {
        if (Microphone.devices.Length == 0) return;
        Microphone.End(null);
        Debug.Log("Start");
        aud.clip = Microphone.Start(null, false, 10, rate);
        
    }

    /// <summary>
    /// 结束录音
    /// </summary>
    public void EndMic()
    {
        int lastPos = Microphone.GetPosition(null);
        if (Microphone.IsRecording(null))
            audioLength = lastPos / rate;//录音时长  
        else
            audioLength = 10;
        Debug.Log("Stop");
        Microphone.End(null);

        clipByte = GetClipData();
        len = clipByte.Length;
        speech = Convert.ToBase64String(clipByte);
        StartCoroutine(GetToken(getTokenAPIPath));
        Debug.Log(len);
        Debug.Log(audioLength);
        
    }

    /// <summary>
    /// 把录音转换为Byte[]
    /// </summary>
    /// <returns></returns>
    public Byte[] GetClipData()
    {
        if (aud.clip == null)
        {
            Debug.LogError("录音数据为空");
            return null;
        }

        float[] samples = new float[aud.clip.samples];

        aud.clip.GetData(samples, 0);


        Byte[] outData = new byte[samples.Length * 2];

        int rescaleFactor = 32767; //to convert float to Int16   

        for (int i = 0; i < samples.Length; i++)
        {
            short temshort = (short)(samples[i] * rescaleFactor);

            Byte[] temdata = System.BitConverter.GetBytes(temshort);

            outData[i * 2] = temdata[0];
            outData[i * 2 + 1] = temdata[1];
        }
        if (outData == null || outData.Length <= 0)
        {
            Debug.LogError("录音数据为空");
            return null;
        }

        //return SubByte(outData, 0, audioLength * 8000 * 2);
        return outData;
    }

    /// <summary>
    /// 获取百度用户令牌
    /// </summary>
    /// <param name="url">获取的url</param>
    /// <returns></returns>
    private IEnumerator GetToken(string url)
    {
        WWWForm getTForm = new WWWForm();
        getTForm.AddField("grant_type", grant_Type);
        getTForm.AddField("client_id", client_ID);
        getTForm.AddField("client_secret", client_Secret);

        WWW getTW = new WWW(url, getTForm);
        yield return getTW;
        if (getTW.isDone)
        {
            if (getTW.error == null)
            {
                token = JsonMapper.ToObject(getTW.text)["access_token"].ToString();
                StartCoroutine(GetAudioString(baiduAPI));
            }
            else
                Debug.LogError(getTW.error);
        }
    }

    /// <summary>
    /// 把语音转换为文字
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator GetAudioString(string url)
    {
        JsonWriter jw = new JsonWriter();
        jw.WriteObjectStart();
        jw.WritePropertyName("format");
        jw.Write(format);
        jw.WritePropertyName("rate");
        jw.Write(rate);
        jw.WritePropertyName("channel");
        jw.Write(channel);
        jw.WritePropertyName("token");
        jw.Write(token);
        jw.WritePropertyName("cuid");
        jw.Write(cuid);
        jw.WritePropertyName("len");
        jw.Write(len);
        jw.WritePropertyName("speech");
        jw.Write(speech);
        jw.WriteObjectEnd();

        WWW getASW = new WWW(url, Encoding.Default.GetBytes(jw.ToString()));
        yield return getASW;
        if (getASW.isDone)
        {
            if (getASW.error == null)
            {
                JsonData getASWJson = JsonMapper.ToObject(getASW.text);
                if (getASWJson["err_msg"].ToString() == "success.")
                {
                    audioToString = getASWJson["result"][0].ToString();
                    if (audioToString.Substring(audioToString.Length - 1) == "，")
                        audioToString = audioToString.Substring(0, audioToString.Length - 1);
                    Debug.Log(audioToString);
                }
                audioToString = null;
            }
            else
            {
                Debug.LogError(getASW.error);
            }
        }
    }

    private void Awake()
    {
        if (GetComponent<AudioSource>() == null)
            aud = gameObject.AddComponent<AudioSource>();
        else
            aud = gameObject.GetComponent<AudioSource>();
        aud.playOnAwake = false;
    }

    //public Text debugText;
    //private void Update()
    //{
    //    debugText.text = audioToString;
    //}
}