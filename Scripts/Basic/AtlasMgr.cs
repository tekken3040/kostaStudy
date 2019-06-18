using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AtlasMgr : Singleton<AtlasMgr>
{
	readonly string SHADER_WHITESCALE = "Sprites/Whitescale";
	readonly string SHADER_GRAYSCALE = "Sprites/Grayscale";
	readonly string SHADER_SPRITES_DEFAULT = "Sprites/Default";
	
	private Dictionary<string, List<Sprite>> mapAtlas = new Dictionary<string, List<Sprite>>(); 
	
	public void Init()
	{
	}
	
	public void SetGrayScale(Image image)
	{
		Material material = new Material(Shader.Find(SHADER_GRAYSCALE));
		image.material = material;
	}

	public void SetWhiteScale(Image image)
	{
		Material material = new Material(Shader.Find(SHADER_WHITESCALE));
		image.material = material;
	}
	
	public void SetDefaultShader(Image image)
	{
		image.material = null;
	}
	
	public Sprite GetSprite(string path)
	{
		string[] splits = path.Split('.');
		
		if(splits.Length < 2)
		{
			DebugMgr.LogError("잘못된 경로 : " + path);
			return null;
		}
		else
		{
			string atlasName = splits[0];
			string spriteName = splits[1];
			
			if(!mapAtlas.ContainsKey(atlasName))
			{
				Object[] objs = AssetMgr.Instance.AssetLoadAll(atlasName, typeof(Sprite));
				
				if(objs == null || objs.Length == 0)
				{
					DebugMgr.LogError("잘못된 경로" + path);
					return null;
				}
				
				List<Sprite> spriteList = new List<Sprite>();
				
				for(int i=0; i<objs.Length; i++)
				{                    
                    if(objs[i].GetType() != typeof(Sprite))  
                        continue;
                    
					spriteList.Add(objs[i] as Sprite);
				}
				
				mapAtlas.Add(atlasName, spriteList);
			}
			
			Sprite sprite = mapAtlas[atlasName].Find((x) => x.name == spriteName);

//			AssetMgr.Instance.UnloadAssetBundleWithFilePath (atlasName + ".png");
//			UnloadAll ();
			
			if(sprite != null)
				return sprite;
			else
			{
				DebugMgr.LogError("잘못된 경로" + path);
				return null;
			}
		}
	}

	public void UnloadAll()
	{
		//SystemInfo.systemMemorySize;
		List<string> keys = mapAtlas.Keys.ToList();

//		for (int i = 0; i < keys.Count; i++) {
//			string val = keys [i];
//			string[] splits = val.Split ('/');
//
//			if (splits.Length > 1 && (splits [1] != "Common" && splits [1] != "Item" && splits [1] != "Tutorial")) {
//				mapAtlas[val].Clear ();
//				mapAtlas.Remove (val);
//			} else if (splits.Length > 2 && (splits [1] == "Item" && splits [2] == "Accessory")) {
//				mapAtlas[val].Clear ();
//				mapAtlas.Remove (val);
//			}
//		}

		for(int i=0; i<keys.Count; i++){
			mapAtlas[keys[i]].Clear ();
			mapAtlas.Remove (keys[i]);
			//DebugMgr.LogError (keys[i]);
//			string[] splits = keys[i].Split ('/');
//			if (splits.Length > 1 && (splits [1] != "Common" && splits [1] != "Item" && splits [1] != "Quest")) {
//				mapAtlas[keys[i]].Clear ();
//				mapAtlas.Remove (keys[i]);
//			}
		}
//		mapAtlas.Clear ();
//		Resources.UnloadUnusedAssets ();
//		mapAtlas.Clear ();
	}
	
	public Sprite GetGoodsIcon(Goods good)
	{
		switch((GoodsType)good.u1Type)
		{
			case GoodsType.GOLD:
			return GetSprite("Sprites/Common/common_02_renew.icon_Gold");
			
			case GoodsType.CASH:
			return GetSprite("Sprites/Common/common_02_renew.icon_Cash");
			
			case GoodsType.KEY:
			return GetSprite("Sprites/Common/common_02_renew.icon_Key");

			case GoodsType.LEAGUE_KEY:
            return GetSprite("Sprites/Shop/shop_02.league_key");
			
			case GoodsType.FRIENDSHIP_POINT:
			return GetSprite("Sprites/Common/common_02_renew.icon_friendship");

            case GoodsType.ODIN_POINT:
            return GetSprite("Sprites/Common/common_04_renew.Icon_Adv_VIP");

            case GoodsType.EQUIP:
			if (good.u2ID == 0) return GetSprite ("Sprites/Item/Item_01.0");
			return GetSprite("Sprites/Item/" + EquipmentInfoMgr.Instance.GetInfo(good.u2ID).cModel.sImagePath);
			
			case GoodsType.MATERIAL:
			return GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(good.u2ID).u2IconID);
			
			
			case GoodsType.CONSUME:
			return GetSprite("Sprites/Item/Item_01." + good.u2ID);
			
			case GoodsType.EQUIP_COUPON:
            return GetSprite("Sprites/Shop/shop_02.equip_drawing");
            break;
			
			case GoodsType.MATERIAL_COUPON:
            return GetSprite("Sprites/Shop/shop_02.stuff_drawing");
            break;
			
			case GoodsType.TRAINING_ROOM:
			break;
			
			case GoodsType.EQUIP_TRAINING:
			break;

            case GoodsType.EQUIP_GOODS:
            return GetSprite("Sprites/Item/" + EquipmentInfoMgr.Instance.GetInfo(EventInfoMgr.Instance.dicClassGoodsEquip[good.u2ID].u2Equip).cModel.sImagePath);

			case GoodsType.EVENT_ITEM:
			return GetSprite("Sprites/Item/Item_01." + EventInfoMgr.Instance.dicMarbleGoods[good.u2ID].u2IconID);

            case GoodsType.CHARACTER_PACKAGE:
                return GetSprite(string.Format("Sprites/Hero/hero_icon.{0}", EventInfoMgr.Instance.dicClassGoods[good.u2ID].u2ClassID));
        }
		
		return null;
	}
	
	public Sprite GetGoodsGrade(Goods good)
	{
		switch((GoodsType)good.u1Type)
		{
			//case GoodsType.GOLD:			
			//case GoodsType.CASH:			
			//case GoodsType.KEY:
			//case GoodsType.LEAGUE_KEY:
			//case GoodsType.FRIENDSHIP_POINT:						
			//case GoodsType.EQUIP_COUPON:
			//case GoodsType.MATERIAL_COUPON:
			//case GoodsType.TRAINING_ROOM:
			//case GoodsType.EQUIP_TRAINING:
			//default:
			//return GetSprite("Sprites/Common/common_02_renew.grade_4571");
			
			case GoodsType.EQUIP:
			case GoodsType.MATERIAL:
			case GoodsType.CONSUME:			
			return GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(good.u2ID));

            case GoodsType.EQUIP_GOODS:
                {
                    ClassGoodsEquipInfo eventEquipInfo;
                    if(EventInfoMgr.Instance.dicClassGoodsEquip.TryGetValue(good.u2ID, out eventEquipInfo))
                    {
                        return GetSprite(string.Format("Sprites/Common/common_02_renew.grade_{0}", (4570 + eventEquipInfo.u1SmithingLevel)));
                    }
                }
                break;
            case GoodsType.CHARACTER_PACKAGE:
                return GetSprite("Sprites/Common/common_02_renew.grade_4570");
        }
		
		return GetSprite("Sprites/Common/common_02_renew.grade_4571");
	}
}
