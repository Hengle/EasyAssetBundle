using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;

namespace charcolle.EasyAssetBundle.Sample {

    public static class EasyAssetBundle {

        public async static Task<T> Load<T>( string resourceName ) where T : UnityEngine.Object {
            return await EasyAssetBundleSampleManager.LoadResource<T>( resourceName );
        }

        public static IObservable<T> ObservableLoad<T>( string resourceName ) where T : UnityEngine.Object {
            return Observable.Create<T>( req => {

                return new CompositeDisposable();
            } );
        }

    }

}