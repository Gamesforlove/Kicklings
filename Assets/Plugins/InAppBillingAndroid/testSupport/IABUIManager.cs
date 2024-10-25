using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class IABUIManager : MonoBehaviourGUI
{

   public string[] products;
#if UNITY_ANDROID
	void OnGUI()
	{
		beginColumn();

		if( GUILayout.Button( "Initialize IAB" ) )
		{
            var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjYOBe6EDzL91jouQMJyjFjGkUenpwghhJepTI2X/Tc4U8rBlJyRh315allHhFE5fommUDc13IWU7iqJx2kvisPZAJ6vAyh93IrkicfsFDNTAhz3ntFJln6rx7iJVn/+0v7u3DvzZRTiTmrYxm6rwd/zdNmV/eq+URFf6xuSS+Y8loMIBHwDB2YFUB5r1+DgU2mzSXtAxJN+NhDr3bQjLfi8/Ie3L8INAz3YkuUNW1v3C0eRoP41f5aUaihRtZFpKVlM2CfS+K6N7/PSVk6MXIyPThJKOubR6dbNFIeE8Y+02uzu4nsYDr8/Xqx+TClTMHXZe/8PPMf+gTVBka18KVQIDAQAB";
			GoogleIAB.init( key );
		}
		
		
		if( GUILayout.Button( "Query Inventory" ) )
		{
			// enter all the available skus from the Play Developer Console in this array so that item information can be fetched for them
            var skus = products;
			GoogleIAB.queryInventory( skus );
		}

        for (int a = 0; a < products.Length; a++) {

            if (GUILayout.Button("Test Purchase_" + products[a]))
            {
                GoogleIAB.purchaseProduct(products[a]);
                
            }		
    }
		
		endColumn( true );

		
		if( GUILayout.Button( "Purchase Real Product" ) )
		{
			GoogleIAB.purchaseProduct( "com.prime31.testproduct", "payload that gets stored and returned" );
		}
		
		
		if( GUILayout.Button( "Consume Real Purchase" ) )
		{
			GoogleIAB.consumeProduct( "com.prime31.testproduct" );
		}

		
		if( GUILayout.Button( "Enable High Details Logs" ) )
		{
			GoogleIAB.enableLogging( true );
		}
		
		
		if( GUILayout.Button( "Consume Multiple Purchases" ) )
		{
			var skus = new string[] { "com.prime31.testproduct", "android.test.purchased" };
			GoogleIAB.consumeProducts( skus );
		}
		
		endColumn();
	}
#endif
}
