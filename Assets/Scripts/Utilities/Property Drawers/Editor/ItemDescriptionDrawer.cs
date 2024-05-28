using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttributer))]
public class ItemDescriptionDrawer :PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //改变返回的属性数值
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();//开始检查改变的值

            var newValue = EditorGUI.IntField(new Rect(position.x,position.y,position.width,position.height/2),label,property.intValue);

            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription
                (property.intValue));

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue=newValue;
            }


        }

        EditorGUI.EndProperty();
    }

    private string  GetItemDescription(int itemCode)
    {
        So_ItemList so_ItemList;

        so_ItemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/So_ItemList.asset", typeof(So_ItemList)) as So_ItemList;

        List<ItemDetails> itemDetailsList = so_ItemList.itemDetails;

        ItemDetails itemDetails = itemDetailsList.Find(x=>x.itemCode == itemCode);

        if (itemDetails != null)
        {
            return itemDetails.itemDescription;
        }
        else
        {
            return "";
        }
    }
}
