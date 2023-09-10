using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoggerManager
{
    private static LoggerManager instance = null;
    private static readonly object padlock = new object();

    public static LoggerManager Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LoggerManager();
                    }
                }
            }
            return instance;
        }
    }

    List<Tuple<string, string>> logHistory = new List<Tuple<string, string>>();

    public void ClearLog()
    {
        logHistory.Clear();
    }

    public void AddLog(string name, string message)
    {
        logHistory.Add(new Tuple<string, string>(name, message));
    }

    public List<Tuple<string, string>> GetLog()
    {
        return logHistory;
    }

    /// <summary>
    /// Logを一つのstringにまとめる
    /// </summary>
    public string GetLogAsSingleString()
    {
        string rtn = string.Empty;
        string lastName = string.Empty;

        for (int i = 0; i < logHistory.Count; i++)
        {
            if (lastName != logHistory[i].Item1)
            {
                rtn += "<alpha=#44><line-height=75%>\n-------------------------------------------------------------\n</line-height><alpha=#FF>"; // 行替え

                // 名前
                if (!(logHistory[i].Item1 == string.Empty))
                {
                    rtn += logHistory[i].Item1;
                    rtn += "<line-height=125%>：";
                    rtn += "\n"; // 行替え
                    lastName = logHistory[i].Item1;
                }
            }
            else
            {
                rtn += "\n";
            }

            // テキスト
            rtn += "</line-height><size=60%><margin=1em>";
            rtn += logHistory[i].Item2;
            rtn += "</size></margin>";
        }

        return rtn;
    }
}