# Shop System
![shopgif2](https://user-images.githubusercontent.com/42221923/144681753-cb2614f9-746b-4e31-9ef5-259dcb1307f9.gif)

Shop is the place where the player can spend the earned coins for cosmetic items, which are Chuck skins, and effects that use particle system.

## How it works

Skins and effects are stored as Scriptable Objects that are assigned to the manager, and instantiated on launch. Custom editor for this Scriptable Object was made for convenience, to show the selected skin in preview.
![image](https://user-images.githubusercontent.com/42221923/144686053-65058175-65a9-4e27-a967-beb6bd662092.png)

To navigate through the items in the shop, ScrollRectSnap script was created that handles the drag behavior. Default drag is handled by the Scroll Rect component, however the script listens to the dragging states in order to snap the Scroll Rect to the closest item after ending the drag.

```c#
public void OnEndDrag
  target = FindNearestIndex(scroll.horizontalNormalizedPosition);
  lerp = true;
  if(content.GetChild(target).childCount > 0)
    content.GetChild(target).GetChild(0).gameObject.SetActive(true);
  UpdateSkinInfo();	
}
```

```c#
int FindNearestIndex(float f){
  float distance = Mathf.Infinity;
  int output = 0;

  for(int i=0; i<points.Length; i++){
    if(Mathf.Abs(points[i]-f) < distance){
      distance = Mathf.Abs(points[i]-f);
      output = i;
    }else{
      break;
    }
  }
  index = output;
  return output;
}
```
After determining the closest item, UI is also updated to show the selected item name and its price. Purchasing or equipping an item triggers the SaveManager to save the information about remaining coins and equipped items.
