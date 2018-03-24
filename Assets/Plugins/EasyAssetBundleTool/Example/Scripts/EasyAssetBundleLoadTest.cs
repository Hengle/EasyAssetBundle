using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    List<string> TestAssetBundleName = new List<string>();

	async void Start () {
        DontDestroyOnLoad( this.gameObject );
        CachePath = Application.streamingAssetsPath + "/" + Platform + "/";
        URL = "";

        await EasyAssetBundleSampleManager.Initialize( data, URL, CachePath, true );

        await TestLoad();
    }

    private async Task TestLoad() {
        // AssetBundleのシーンのロード
        // AssetBundleをメモリに展開しておけばSceneManager.LoadSceneで読み込める
        var test = await EasyAssetBundle.LoadAssetBundle( "prefab" );
        Debug.Log( test );
        var prefab = await EasyAssetBundle.Load<GameObject>( "Cube" );
        Instantiate( prefab );
        var test2 = await EasyAssetBundle.LoadAssetBundle( "scene1" );
        SceneManager.LoadScene( "scene1", LoadSceneMode.Additive );
        var test3 = await EasyAssetBundle.LoadAssetBundle( "scene2" );
        SceneManager.LoadScene( "scene2", LoadSceneMode.Additive );
        //var obj = GameObject.Find( "Cube" );
        //obj.GetComponent<MeshRenderer>().materials[ 0 ] = material;
    }

}
