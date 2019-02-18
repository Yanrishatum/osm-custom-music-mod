using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

namespace CustomMusicMod
{
  public class CMM
  {

    // Api entry-points
    // TODO: Subfolder support
    static Dictionary<string, CMMSong> songs = new Dictionary<string, CMMSong>();

    public static T[] LoadSonglist<T>()
    {
      List<T> results = new List<T>();
      results.AddRange(JsonConvert.DeserializeObject<T[]>(Resources.Load<TextAsset>("songs").text));
      LoadSonglistRec(Util.ModsPath(), results);
      
      return results.ToArray();
    }

    static void LoadSonglistRec<T>(string mpath, List<T> results)
    {
      foreach(string dir in Directory.GetDirectories(mpath))
      {
        LoadSonglistRec(Path.Combine(mpath, dir), results);
      }
      foreach (string file in Directory.GetFiles(mpath, "*.json"))
      {
        string data = File.ReadAllText(Path.Combine(mpath, file));
        if (data[0] == '[')
        {
          T[] custom = JsonConvert.DeserializeObject<T[]>(data);
          results.AddRange(custom);
          CMMSong[] cmm = JsonConvert.DeserializeObject<CMMSong[]>(data);
          foreach (CMMSong song in cmm)
          {
            song.Path = Path.Combine(mpath, song.Id);
            songs.Add(song.Id, song);
            //Debug.Log(song);
          }
        }
        else if (data[0] == '{')
        {
          T custom = JsonConvert.DeserializeObject<T>(data);
          results.Add(custom);
          CMMSong cmm = JsonConvert.DeserializeObject<CMMSong>(data);
          cmm.Path = Path.Combine(mpath, cmm.Id);
          //Debug.Log(cmm);
          songs.Add(cmm.Id, cmm);
        }
      }
    }
   
    public static SongData LoadCustomSong(string id, AssetBundle bundle)
    {
      if (Util.IsCustomSong(id))
      {
        return new SongData(id);
      }
      return new SongData(id, bundle);
    }
    
    public static AudioClip LoadCustomPreview(string id)
    {
      if (Util.IsCustomSong(id))
      {
        return LoadPreview(id);
      }
      return Resources.Load<AudioClip>("songs-preview/" + id);
    }

    // Generic fuckery to avoid circular reference, yay.
    // Returns variation of a custom song so arcade wouldn't crash on trying to launch song that have no attached challenge.
    // Challenge map determined by first character.
    public static T GetVariation<T>(Dictionary<T, string> dict, string id)
    {
      if (dict.ContainsValue(id))
        return dict.First(x => x.Value == id).Key;
      CMMSong song;
      if (songs.TryGetValue(id, out song))
      {
        if (song.Challenge.HasValue)
          return dict.ElementAt(song.Challenge.Value % dict.Count).Key;
        if (song.ChallengeName != null && dict.ContainsValue(song.ChallengeName))
        {
          return dict.First(x => x.Value == song.ChallengeName).Key;
        }
      }
      return dict.ElementAt(id[0] % (dict.Count)).Key;
    }

    // Actual loading

    public static string LoadSheet(string id)
    {
      return File.ReadAllText(Util.SelectSheet(id));
    }

    public static AudioClip LoadSong(string id)
    {
      return LoadAudioClip(Util.SongPath(id));
    }
    
    public static AudioClip LoadPreview(string id)
    {
      return LoadAudioClip(Util.PreviewPath(id));
    }

    // Let the insanity ensure
    static AudioClip LoadAudioClip(string path)
    {
      var req = LoadAudioClipWait(path);
      while (req.MoveNext())
        if (req.Current != null) break;
      return req.Current;
    }

    static IEnumerator<AudioClip> LoadAudioClipWait(string path)
    {
      using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, Util.ToAudioType(path)))
      {
        var async = www.SendWebRequest();
        while (!async.isDone) yield return null;
        yield return DownloadHandlerAudioClip.GetContent(www);
      }
    }

  }
  
  public class SongData
  {
    public string id;
    AssetBundle bundle;
    string sheetName;
    string arcade;
    string story;

    public SongData(string id)
    {
      this.id = id;
    }

    public SongData(string id, AssetBundle bundle)
    {
      this.id = id;
      this.bundle = bundle;
      string[] names = bundle.GetAllAssetNames();
      sheetName = names.FirstOrDefault<string>(x => x.Contains(id + ".sheet.txt"));
      arcade = names.FirstOrDefault<string>(x => x.Contains(id + ".ogg") || x.Contains(id + ".mp3"));
      story = names.FirstOrDefault<string>(x => x.Contains(id + ".story.ogg") || x.Contains(id + ".story.mp3"));
      if (story == null) story = arcade;
    }

    public string GetSheet()
    {
      if (bundle != null)
      {
        return bundle.LoadAsset<TextAsset>(sheetName).text;
      }
      return CMM.LoadSheet(id);
    }

    public AudioClip GetClip(bool storyMode)
    {
      if (bundle != null)
      {
        return bundle.LoadAsset<AudioClip>(storyMode ? story : arcade);
      }
      return CMM.LoadSong(id);
    }
    
  }

  public class CMMSong
  {

    public string Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public int EasyGrade { get; set; }
    public int NormalGrade { get; set; }
    public int HardGrade { get; set; }
    public int? UberGrade { get; set; }
    public int CurrentVersion { get; set; }
    public string Duration { get; set; }
    public string[] Albums { get; set; }
    public int? Challenge { get; set; }
    public string ChallengeName { get; set; }
    public string Path { get; set; }

    override public string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

  }

  class Util
  {
    // Base folder for mod contents
    static readonly string MOD_FOLDER_NOSLASH = "cmm";
    public static readonly string MOD_FOLDER = MOD_FOLDER_NOSLASH + "/";
    //private static readonly string ALBUM_FOLDER = MOD_FOLDER + "albums/";
    // Supported formats. No guarantee it'll work with anything bug ogg/wav tho.
    private static readonly string[] SONG_FORMATS = new string[] { ".ogg", ".wav", ".it", ".s3m", ".xm", ".mod" };
    private static readonly AudioType[] SONG_TYPES = new AudioType[] { AudioType.OGGVORBIS, AudioType.WAV, AudioType.IT, AudioType.S3M, AudioType.XM, AudioType.MOD };
    
    public static string AppPath
    {
      get
      {
        return new Regex(@"(?'path'.*\/)[^\$]").Match(Application.dataPath).Groups["path"].Captures[0].Value;
      }
    }
    
    // Returns path to mods folder
    public static string ModsPath()
    {
      return Path.Combine(AppPath, MOD_FOLDER_NOSLASH);
    }

    // Returns either custom sheet or default one.
    public static string SelectSheet(string id)
    {
      string custom = CustomSheetPath(id);
      if (File.Exists(custom)) return custom;
      return SheetPath(id);
    }

    // Returns path to sheet file
    public static string SheetPath(string id)
    {
      return Path.Combine(AppPath, MOD_FOLDER + id + ".sheet.txt");
    }

    // Returns path to custom sheet file (Editor generated)
    public static string CustomSheetPath(string id)
    {
      return Path.Combine(AppPath, "customsheets/" + id + ".sheet.txt");
    }

    // Returns path to a song file.
    public static string SongPath(string id)
    {
      string basePath = Path.Combine(AppPath, MOD_FOLDER + id);
      if (File.Exists(basePath)) return basePath;
      foreach (string ext in SONG_FORMATS)
        if (File.Exists(basePath + ext)) return basePath + ext;
      Debug.LogWarning("Couldn't find song file for: " + id);
      return basePath + ".song";
    }

    // Returns path to a song preview file (or song file, if none present)
    public static string PreviewPath(string id)
    {
      string basePath = Path.Combine(AppPath, MOD_FOLDER + id + "-preview");
      if (File.Exists(basePath)) return basePath;
      foreach (string ext in SONG_FORMATS)
        if (File.Exists(basePath + ext)) return basePath + ext;
      //Debug.LogWarning("Couldn't find preview song for: " + id);
      return SongPath(id);
    }

    // Checks if song ID is a custom song or built-in.
    public static bool IsCustomSong(string id)
    {
      return File.Exists(SheetPath(id)) && File.Exists(SongPath(id));
    }

    public static AudioType ToAudioType(string path)
    {
      string ext = Path.GetExtension(path);
      for (int i = 0; i < SONG_FORMATS.Length; i++)
      {
        if (ext == SONG_FORMATS[i])
        {
          return SONG_TYPES[i];
        }
      }
      return AudioType.UNKNOWN;
    }

  }

}
