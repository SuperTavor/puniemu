import requests
import json
import os
from sharedLogic.NHN import *
try:
    from google_play_scraper import app as AVD # Automatic-Version-Detector
except:
    pass
class GamedataDownloader:
    def __init__(self, inputUdkey, debug = False, override = None): # inputUdkey (udkey to use) | debug (print debug info) | override (path where get files)
        # Set some value
        self.debug = debug
        self.token = None
        self.apikey = "a-zrhgm09pgcgjc1iv9cxvpk3xm9b0ynyo4u00sny6bjq10nrx3up2yrhjnq2lhg"
        self.mstVersionVer = 0
        self.dumped_key = []
        self.files = {}
        self.userId = None
        
        # Ask Output 
        output_folder = input("Output folder: ")
        os.makedirs(output_folder, exist_ok=True)
        
        # Get App Version
        try:
            res = AVD("com.Level5.YWP")
            self.appVer = res['version']
            if (self.debug):
                print(f"[DEBUG] Detected version : {self.appVer}")
        except:
            if (self.debug):
                print(f"[DEBUG] Automatic Version Detector - Failed")
            self.appVer = input("Game version to spoof (latest version only): ").strip()
        
        # Get Udkey and Gdkey (with signature et other information)
        instance = self.get_active(inputUdkey)
        if (instance == None):
            if (self.debug):
                print(f"[DEBUG] Failed with the udkey")
            return
        self.gdkey = instance["gdkeys"][0]["value"]
        self.udkey = instance["udkey"]["value"]
        
        if (override != None):
            if (debug):
                print(f"[DEBUG] Dumping override (path='{override}')")
            override_files = [(f, os.path.splitext(f)[0]) for f in os.listdir(dossier) if os.path.isfile(os.path.join(override, f))]
            for i in override_files:
                self.dumped_key.append(i[1])
                try:
                    self.files[i[1]] = json.load(open(i[0], "r", encoding = "utf-8"))
                except:
                    with open(i[0], "r", encoding = "utf-8") as f:
                        self.files[i[1]] = f.read()
                
        # Get all tables and needed keys
        self.extract_response(self.get_request("init.nhn"), ["hitodamaShopSaleList","shopSaleList","ymoneyShopSaleList","noticePageList","mstVersionMaster"])
        self.extract_response(self.get_request("getMaster.nhn", udkey = self.udkey, custom_req = {"tableNames":"all"}))
        self.extract_response(self.get_request("login.nhn", udkey = self.udkey, gdkey = self.gdkey, custom_req = {"batteryInfo":{"level":85,"state":2,"technology":"Li-ion","temperature":328,"voltage":3500},"deviceName":"z3q","gdkeySignature":instance["gdkeys"][0]["signature"],"isL5IDLinked":0,"modelName":"SM-G988N","gdkeyValue":self.gdkey,"signNonce":instance["sign_nonce"],"signTimestamp":instance["sign_timestamp"],"signature":"s4X9CoyxGma3kGuAp5woThgvBX3dCi77Slh5RcOo6ybmMTt0J4CGiZwyiCsil7P3MVgjiVt+kGE1MqvttCXLB+hlOpyTkJp5a78TXthBNVw=","udkeySignature":instance["udkey"]["signature"],"udkeyValue":self.udkey}))
        user_tables_list = ["ywp_user_stage_rank", "ywp_user_hist_puzzle_daily", "ywp_user_hist_youkai_total", "ywp_user_box_reward", "ywp_user_local_event_movie_viewed", "ywp_user_hist_total", "ywp_user_youkai_collect", "ywp_user_local_hpatk_cutin", "ywp_user_friend_raid_boss", "ywp_user_local_oni_cutin_viewed", "ywp_user_gacha", "ywp_user_event_area_assist", "ywp_user_event_condition", "ywp_user_score_attack_reward", "ywp_user_self_rank_event", "ywp_user_friend_rank_event", "ywp_user_mission", "ywp_user_local_player_icon_select", "ywp_user_local_player_plate_select", "ywp_user_youkai_bonus_effect", "ywp_user_present_box_list", "ywp_user_youkai_legend_release_history", "ywp_user_local_shop_item_unlock", "ywp_user_present_box", "ywp_user_local_player_codename_select", "ywp_user_tournament_message", "ywp_user_player_plate", "ywp_user_local_stage_searched", "ywp_user_local_treasure_series_unlocked", "ywp_user_goku_youkai_intro_release", "ywp_user_dictionary", "ywp_user_item", "ywp_user_production", "ywp_user_youkai_skill", "ywp_user_watch", "ywp_user_local_gacha_tutorial_task_kill_status", "ywp_user_event_point", "ywp_user_campaign", "ywp_user_raid_boss", "ywp_user_crystal_menu", "ywp_user_local_watch_select", "ywp_user_medal_point_trade", "ywp_user_local_crystalyouki_appearance", "ywp_user_mini_game_map_friend", "ywp_user_local_event_progress_status", "ywp_user_event_tutorial", "ywp_user_steal_progress", "ywp_user_local_gokuyouki_appearance", "ywp_user_hist_youkai_daily", "ywp_user_event_quest", "ywp_user_youkai", "ywp_user_drive_progress", "ywp_user_youkai_medal_cnt", "ywp_user_raid_boss_attack", "ywp_user_player_effect", "ywp_user_local_medal_trade_chked", "ywp_user_icon_budge", "ywp_user_player_codename", "ywp_user_event_ranking_reward", "ywp_user_local_shop_item_select", "ywp_user_local_raid_boss_cutin_viewed", "ywp_user_goku_story", "ywp_user_local_player_title_select", "ywp_user_local_limitedpack_chked", "ywp_user_score_attack_point_trade", "ywp_user_shop_item_unlock", "ywp_user_gacha_stamp", "ywp_user_local_acore_attack_trade_chked", "ywp_user_local_watch_unlock", "ywp_user_event", "ywp_user_treasure", "ywp_user_hist_puzzle_weekly", "ywp_user_all_rank_event", "ywp_user_player_title", "ywp_user_friend_star_rank", "ywp_user_stage", "ywp_user_youkai_deck", "ywp_user_menufunc", "ywp_user_self_rank", "ywp_user_shop_item_remain_cnt", "ywp_user_local_webpage_chked", "ywp_user_event_progress", "ywp_user_conflate", "ywp_user_youkai_strong_skill", "ywp_user_shoot_message", "ywp_user_friend", "ywp_user_local_player_effect_select", "ywp_user_treasure_series", "ywp_user_map", "ywp_user_login_stamp_list", "ywp_user_friend_rank", "ywp_user_friend_dictionary_rank", "ywp_user_local_event_trade_chked", "ywp_user_local_youkai_new", "ywp_user_local_youkaicollect_appearance", "ywp_user_event_area_present", "ywp_user_event_message", "ywp_user_local_event_help_chked", "ywp_user_ads_play", "ywp_user_league_rank", "ywp_user_mini_game_map", "ywp_user_friend_request_recv", "ywp_user_friend_stage", "ywp_user_tutorial_list", "ywp_user_youkai_intro", "ywp_user_data", "ywp_user_event_point_trade", "ywp_user_local_appearance_status", "ywp_user_local_kimagure_map_cutin_viewed", "ywp_user_player_icon", "ywp_user_all_rank", "ywp_user_local_event_stage_searched", "ywp_user_local_item_select", "ywp_user_box_reward", "ywp_user_hist_total", "ywp_user_gacha", "ywp_user_mission", "ywp_user_player_plate", "ywp_user_item", "ywp_user_dictionary", "ywp_user_production", "ywp_user_youkai_skill", "ywp_user_watch", "ywp_user_event_point", "ywp_user_campaign", "ywp_user_raid_boss", "ywp_user_youkai", "ywp_user_player_effect", "ywp_user_icon_budge", "ywp_user_gacha_stamp", "ywp_user_treasure", "ywp_user_stage", "ywp_user_player_title", "ywp_user_youkai_deck", "ywp_user_menufunc", "ywp_user_conflate", "ywp_user_map", "ywp_user_ads_play", "ywp_user_mini_game_map", "ywp_user_tutorial_list", "ywp_user_player_icon"]
        self.extract_response(self.get_request("userInfoRefresh.nhn", udkey = self.udkey, gdkey = self.gdkey, userId = self.userId, custom_req = {"activeDeckId":1,"requireInfoList":user_tables_list}))
        self.extract_response(self.get_request("initGacha.nhn", udkey = self.udkey, gdkey = self.gdkey, userId = self.userId, custom_req = {"activeDeckId":1}), custom_data = ["gachaStampList","gachaStampIdList","gachaStampRewardList","canPossessionItemList","gachaLotRuleList","canUseMultiGachaCoinIdList","bannerResourceList"])
        
        # Create : Stage Dump
        self.create_stage_dump(json.load(open("level_data_dump_info.txt", "r", encoding = "utf-8")))
        
        # Save all files
        for key, value in self.files.items():
            if (type(value) == list or type(value) == dict):
                json.dump(value,open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8"),ensure_ascii=False)
            else:
                with open(os.path.join(output_folder, f"{key}.txt"), "w",encoding="utf-8") as f:
                    f.write(str(value))
            if (debug):
                print(f"[DEBUG] Saved file : {key}.txt")
                
    def get_request(self, url_endpoint, udkey = None, userId = None, custom_req = {}, gdkey = None):
        # Send requests to the game server
        
        if (self.debug):
            print(f"[DEBUG] Requesting : {url_endpoint}")
        req = {"appVer":self.appVer, "mstVersionVer":self.mstVersionVer,"osType":2,"ywpToken":"0","deviceId":"0","level5UserId":"0","userId":"0"}
        if (udkey != None):
            req["deviceId"] = udkey
        if (gdkey != None):
            req["level5UserId"] = gdkey
        if (userId != None):
            req["userId"] = userId
        if (custom_req != None and type(custom_req) == dict):
            req = {**req,**custom_req}
        if (self.token != None):
            req["token"] = self.token
        try:
            response = requests.post(GAMESERVER+url_endpoint, headers = headers, data = encrypt_req(json.dumps(req, ensure_ascii = False)))
        except:
            return None
        if (response.status_code != 200):
            return None
        decrypted_res = json.loads(decrypt_res(response.text))
        if (decrypted_res.get("resultCode") != 0):
            return None
        if (decrypted_res.get("token") != None):
            self.token = decrypted_res.get("token")
        if (decrypted_res.get("mstVersionMaster") != None):
            self.mstVersionVer = decrypted_res.get("mstVersionMaster")
        if (decrypted_res.get("ywp_user_data") != None):
            self.userId = decrypted_res.get("ywp_user_data").get("userId")
        return decrypted_res
    
    def get_active(self, udkey):
        # Get l5id credentials
        
        base_uri = f"https://api.level5-id.com/api/v1/active/?apkey={self.apikey}&device_cd=star2gltechn_9&device_type_cd=Android&sign=true&udkey={udkey}&version={self.appVer}"
        try:
            r = requests.get(base_uri)
        except:
            return None
        if (r.status_code != 200):
            return None
        try:
            r = json.loads(r.text)
            if (r.get("result") == True):
                return r
            return None
        except:
            return None
        
    def extract_response(self, resp, custom_data = []):
        # Extract ywp_user, ywp_mst, and extra keys from response
        
        files = {}
        if (resp == None):
            if (self.debug):
                print("[DEBUG] Error bad response")
            raise Exception("Download Failed")
        for key, value in resp.items():
            if (key in self.dumped_key):
                continue
            if (key in custom_data):
                files[key] = value
                self.dumped_key.append(key)
            elif ("ywp_mst" in key):
                obj = value
                self.dumped_key.append(key)
                """if (type(obj) == list):
                    obj = {"data":obj}
                elif (type(obj) == str):
                    obj = {"tableData":obj}
                elif (type(obj) == dict and len(obj) == 1 and obj.get("data") == None):
                    obj = {"data":obj}"""
                files[key] = obj
            elif ("ywp_user" in key):
                files[key+"_def"] = value
                self.dumped_key.append(key)
        self.files = {**self.files, **files}
        
    def create_stage_dump(self, value_dump):
        # Create Stage Dump
        if (self.debug):
            print(f"[DEBUG] Generating stage dump file")
        fight_dump = {}
        stages_list = self.files["ywp_mst_stage"]["tableData"].split("*")
        const = value_dump
        for i in stages_list:
            element = i.split("|")
            stageId = int(element[0])
            setId = int(element[12])
            fight_dump[stageId] = {}
            fight_dump[stageId]["enemy"] = []
            try:
                fight_dump[stageId]["tutorial_edit"] = const[str(stageId)]["tutorial_edit"]
            except:
                fight_dump[stageId]["tutorial_edit"] = {"requests" : None, "response" : None}
            try:
                fight_dump[stageId]["menufunc_edit"] = const[str(stageId)]["menufunc_edit"]
            except:
                fight_dump[stageId]["menufunc_edit"] = []
            try:
                fight_dump[stageId]["first_reward"] = const[str(stageId)]["first_reward"]
            except:
                fight_dump[stageId]["first_reward"] = []
            enemyId = -1
            set_list = self.files["ywp_mst_youkai_enemy_set"]["tableData"].split("*")
            enemy_param = self.files["ywp_mst_youkai_enemy_param"]["tableData"].split("*")
            defaultBefriends = 0
            try:
                defaultBefriends = const[str(stageId)]["default_befriends"]
            except:
                defaultBefriends = 0
            for i2 in set_list:
                element2 = i2.split('|')
                if (int(element2[0]) == setId):
                    enemyId = int(element2[1])
                    break
            if (enemyId != -1):
                if (defaultBefriends == enemyId):
                    fight_dump[stageId]["enemy"].append({"id":enemyId,"defBefriends":1})
                else:
                    fight_dump[stageId]["enemy"].append({"id":enemyId,"defBefriends":0})
                for i3 in range(2):
                    nextEnemy = -1
                    for i2 in enemy_param:
                        param_element = i2.split("|")
                        if (int(param_element[0]) == enemyId+(i3+1)):
                            nextEnemy = enemyId+(i3+1)
                            break
                    if (nextEnemy != -1):
                        if (defaultBefriends == nextEnemy):
                            fight_dump[stageId]["enemy"].append({"id":nextEnemy,"defBefriends":1})
                        else:
                            fight_dump[stageId]["enemy"].append({"id":nextEnemy,"defBefriends":0})
                    else:
                        break
        self.files["stage_data"] = fight_dump        

GamedataDownloader("d-k3l6lh7d0jtafxq3ez5twlf5t1c4mcbwx905t6y8qwg50siubxj2yrkriq2lhf", debug = True)


