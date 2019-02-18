# Old School Musical: Custom Music Mod
TODO: Put some description here

## Installation
* Put files into OSM installation directory
* Make sure that you replaced `Assembly-CSharp.dll` and put `CustomMusicMod.dll` to `osm_data/Managed` folder.
* Make sure you have `<osm folder>/cmm` folder present.

## Usage
* Place custom song files into `cmm` folder.
* Play it from arcade.

## Making your own songs

### Custom song format
Each custom song follows next format:
```
<manifest>.json
<id>.ogg
<id>-preview.ogg
<id>.sheet.txt
```
* Manifest and sheet explained in detail below.
* Supported sound formats are: OggVorbis (`.ogg`), ScreamTracker3 (`.s3m`), ImpulseTracker (`.it`) and ProTracker/FastTracker files (`.mod`). Mp3 is not supported (blame Unity).
* `-preview` file used when you select song in the arcade list. Supposed to be a short snippet. It's optional, because mod will load full song if it isn't present, but preferable.


### Manifest format
Manifest file should describe the songs it will add. It's actual name is irrelevant but should always have an extension of `.json`
```json
[
  { "Id":"song id","Title":"song title","Artist":"song author","EasyGrade":1,"NormalGrade":5,"HardGrade":8,"UberGrade":null,"CurrentVersion":1,"Duration":"00:01:39","Albums":["legendsofosm"], "ChallengeName": "solitude" }
]
```
If manifest file describes only one file, array declaration can be omitted. 
* `Id` should point to the name of the new songs and will be used for arcade rating.
* `Title` and `Artist` represent title and author of the song. (duh)
* `EasyGrade`, `NormalGrade` and `HardGrade` represent estimated difficulty of the song shown in song list (1-10). `UberGrade` seem to be initially planned difficulty in the game but scrapped at some point (and thank God).
* `CurrentVersion` - unusued, remain at 1.
* `Duration` - Duration of the song that would be shown in song list in format `HH:MM:SS`.
* `Albums` - an array of albums this song belongs to. It's not possible to create new albums, because technical limitations, all album ids listed below. At least one album should be present. If albums listed do not exist, track still will be available trough all songs cassette.  
  List of visible albums: `talesofosm`, `chickenpartymix`, `yponeko`, `dubmood`, `zabutom`, `helloworld`, `leplancton`
* `ChallengeName` - optional. Allows to set specific challenge background to the song. See list below for possible values. If not set, mod will use charcode of first letter in song ID to determine challenge index.
  <details open><summary>List of songs</summary>
  
  * Challenge (in-game number): `ID`
  * 1: `runningthroughfields`
  * 2: `matriarchy`
  * 3: `magicalflute`
  * 4: `iamready`
  * 5: `yourewelcome`
  * 6: `overthesky`
  * 7: `major9`
  * 8: `keygen20`
  * 9: `ocean`
  * 10: `fireworks`
  * 11: `korobeiniki`
  * 12: `megalonaan`
  * 13: `kurakura`
  * 14: `watermelons`
  * 15: `rezcracktro4`
  * 16: `keygen21`
  * 17: `bandana`
  * 18: `capriccio`
  * 19: `bloodybeaks`
  * 20: `cobblesofhell`
  * 21: `marmite`
  * 22: `godotvalley`
  * 23: `savat`
  * 24: `runforjoy`
  * 25: `confucius`
  * 26: `balladcoolfish`
  * 27: `naive`
  * 28: `settlers7`
  * 29: `keygen23`
  * 30: `mega23`
  * 31: `ghostghost`
  * 32: `keygen22`
  * 33: `8bitdojo`
  * 34: `keygen19`
  * 35: `nefariouscrown`
  * 36: `datasalen`
  * 37: `isocity`
  * 38: `blackhattower`
  * 39: `lobsterloser`
  * 40: `drugedmj`
  * 41: `powerwithouttheprice`
  * 42: `hundreddollar`
  * 43: `makingmusic`
  * 44: `lovesong`
  * 45: `badtelevision`
  * 46: `gameovercity`
  * 47: `ststylemedley`
  * 48: `losatankar`
  * 49: `enemiescloser`
  * 50: `solitude`
    
  </details>

### Sheet format
TODO. Format is pretty simple.
```
#<name>
@<id>
Offset <offset>
Bpm <bpm>
# Easy (<marge>)
<easy sheet>
# Normal (<marge>)
<normal sheet>
# Hard (<marge>)
<hard sheet>
```
* `BU`, `BD`, `BL`, `BR`, `TL`, `TR`
* id/name irrelevant
* Not sure how offset works.
* Bpm should be accurate
* marge - lower value = faster notes, min 0, max 9.999(9). Most likely in seconds.
```
<beat> <button> [press duration in beats]
4 BR
4.25 BD 0.25
6 TL
6 TR
```

### Built-in editor
You can unlock built-in editor by placing `editor.unlock` file in the game folder.  
This editor was made by developers and pretty rough, plus I suspect at some point they switched to different editor.

> **Important**: Editor have a memory leak, and I advice using it only if you have at least 12GB ram and restart the game every ~15-30 minutes. I take no responsibility for OS issues you'll have if you'll run out of RAM due to that.

Edited note sheets will be placed by the game at `<osm folder>/customsheets/<id>.sheet.txt`. 

#### Editor controls
* <kbd>Space</kbd> : Play/Pause
* <kbd>Num8</kbd>, <kbd>Num2</kbd>, <kbd>Num4</kbd>, <kbd>Num6</kbd> : Place Up, Down, Left and Right notes respectively.
* <kbd>Num7</kbd> and <kbd>Num9</kbd> : Place left and right triggers.
* <kbd>Num *</kbd> : Convert last note to press-and-hold note and put it's end at current tick.
* <kbd>Num /</kbd> : While held, pressing a direction will convert last note in that direction to press-and-hold note and put it's end at current tick.
* <kbd>Up</kbd> and <kbd>Down</kbd> : Step one tick back/forward.
* <kbd>Left</kbd> and <kbd>Right</kbd> : Navigation in the song.
* <kbd>Page Up</kbd> and <kbd>Page Down</kbd> : Increase/decrease amount of beat divisors.

Fun fact: There also `TickButtonAppearDown/Left/Right/Up`, `TickSphereDown/Left/Right/Up` `AlternateTickSphereMode` and `RotateTickButtonLeft/Righ` actions. And I have no clue if they even used, because they aren't bound directly to hotkeys.


## Future plans
If this will gain traction, I will try to implement those features.

* Support for subfolders for better custom song organisation.
* Version-independed automatic patching of the `Assembly-CSharp.dll`.
* More control over song parameters.

### Not planned
* Custom albums in the list.  
I won't do it, because ArcadeK7 album instances are created by Unity directly and I don't want to bother with code-side adding of them at runtime.
* Non-leaky editor.  
Why would I fix it? I don't even know what causing the leak in the first place. :)
* External editor, since editor is leaky.  
Too much effort.

## Known issues
* If game updates, mod will stop working, duh.
* Make sure you have a few seconds without notes at song start, because game drops inputs for first ~4 seconds for some reason (Isn't the case for editor!, probably because of transition effects). Also at the end as well just in case.

## How the hell?
Aka entry-points. This is more notes to myself on how to reproduce the mod by manually patching `Assembly-CSharp.dll`.

```bash
# Load proper album data. Scans cmm folder on top of loading base-game files.
@method SongDatabase::LoadAlbumsData():Void
# Song[] source = CMM.LoadSonglist();

# Show all arcade songs. This is a bit of a cheatcode, because it unlocks all the songs, but it's easiest way to get custom songs show up in arcade.
@method Menus.Arcade.ChooseAlbumManager.<Awake>m__1(Song):Boolean
@method Menus.Arcade.SongList.<SetAlbum>m__3(Song):Boolean
@method Menus.Arcade.SongList.<SetAlbum>m__4(Song):Boolean
# return id != "mountainstage";

# Properly play preview song. Load it from bundle for stock songs and from file for custom ones.
@method Menus.Arcade.SongList.PlaySongPreview():Void
# AudioClip clip = CMM.LoadCustomPreview(this.SelectedSong.Id);

# Allow loading of arcade song. Prevents error on song start, because custom songs aren't hardcoded into the game to have specific stage assigned to it.
@method LevelLoader.SetVariation(String):Void
# RpgIsoLevelManager.CurrentLevelVariation = CMM.GetVariation<RpgIsoLevelManager.LevelVariation>(RpgIsoLevelManager.ChallengeSongs, songId);

# Redirect loading of music data to mod. Replaces entire method, because I reproduced this function with custom music support in my code, because it's more sane solution.
@method SongDataContainer::GetSongData(String):SongDataContainer
# SongDataContainer container = new SongDataContainer();
# CustomMusicMod.SongData data = CustomMusicMod.CMM.LoadCustomSong(songId, DataManager.Instance.SongToPlay.Bundle);
# container.Sheet = MsfConverter.GetSheetFromMsf(data.GetSheet());
# container.Clip = data.GetClip(DataManager.Instance.CurrentGameMode == DataManager.GameMode.Story);

# Don't crash if there's no bundle. Well. Duh. Custom songs don't have asset bundles. And release game can't have this issue, because all songs are already present.
@method Song.<LoadBundleToMemory>c__Iterator0.MoveNext():Boolean
# Instead of throw -> return true
```