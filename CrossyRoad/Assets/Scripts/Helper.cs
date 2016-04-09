using UnityEngine;
using System.Collections;

public class Helper{
	public static T GetRandomEnum<T>() // for randoming enumeration
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}
}
