using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainColors : MonoBehaviour {

	public static Color32 Transparent()
	{
		return new Color32(255, 255, 255, 0);
	}
	
	public static Color32 White()
	{
		return new Color32(255, 255, 255, 255);
	}
	
	public static Color32 WhiteTransparent(byte transparency)
	{
		return new Color32(255, 255, 255, transparency);
	}
	
	public static Color32 Black()
	{
		return new Color32(0, 0, 0, 255);
	}
	
	public static Color32 Grey()
	{
		return new Color32(150, 150, 150, 255);
	}
	
	public static Color32 GreenLight()
	{
		return new Color32(100, 240, 0, 255);
	}
	
	public static Color32 GreenDark()
	{
		return new Color32(90, 175, 75, 255);
	}
		
	public static Color32 Red()
	{
		return new Color32(255, 20, 30, 255);
	}	
	
	public static Color32 Orange()
	{
		return new Color32(255, 100, 20, 255);
	}
		
	public static Color32 Yellow()
	{
		return new Color32(255, 220, 20, 255);
	}
	
	public static Color32 ToggleIsOn()
	{
		return new Color32(60, 120, 0, 255);
	}
	
	public static Color32 SideSelectionShadow()
	{
		return new Color32(22, 19, 12, 255);
	}

}
