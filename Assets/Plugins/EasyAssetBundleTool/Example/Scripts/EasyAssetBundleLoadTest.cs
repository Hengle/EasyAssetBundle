using UnityEngine;

using charcolle.EasyAssetBundle.Sample;

/// <summary>
/// INPROGRESS!!!!!!!!!!!!!!!!!!!!!! : (
/// </summary>
public class EasyAssetBundleLoadTest : MonoBehaviour {

    [SerializeField]
    string CachePath;
    [SerializeField]
    string URL;
    [SerializeField]
    string Platform;
    [SerializeField]
    SampleAssetBundleInfoData data;

    [Space( 10f )]
    [SerializeField]
    string TestAssetBundleName;

	async void Start () {

        CachePath = Application.streamingAssetsPath + "/" + Platform + "/";
        URL = "";

        await EasyAssetBundleSampleManager.Initialize( data, URL, CachePath );

        var test = await EasyAssetBundle.Load<GameObject>( TestAssetBundleName );
        Debug.Log( test );
    }

}
