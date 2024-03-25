using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionNumber : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<TMP_Text>().text = "ver " + Application.version;
    }
}
