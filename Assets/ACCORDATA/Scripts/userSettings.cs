using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum languages { eng, zhTw };
public class userSettings : MonoBehaviour
{
    [Header("--> ACCORDATA <--")]
    public languages language = languages.eng;

}
