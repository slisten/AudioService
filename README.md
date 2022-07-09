# AudioService

简单的Unity音频服务工具

## 安装说明
可以直接复制源代码到自己项目或者自己编译dll

## Example

    public class CusAudioLoader:IAudioLoader
    {
        public AudioClip LoadSound(string path)
        {
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        public void LoadSoundAsync(string path, Action<AudioClip> onComplete)
        {
            
        }

        public void UnloadSound(string path)
        {
            
        }
    }

    AudioService.Init(new CusAudioLoader());
    AudioService.PlaySound("Assets/XResoures/Sounds/bgm/bgm_battle02_1.ogg",SoundType.BGM,true);
    AudioService.PlaySound("Assets/XResoures/Sounds/cv/emilia/jp_st_emilia_0024.ogg",SoundType.Voice);
    AudioService.PlaySound("Assets/XResoures/Sounds/story/sound_cantopendoor.ogg",SoundType.UI);
    AudioService.PlaySound("Assets/XResoures/Sounds/interactive/act01bear/sound_act01bear_dash.ogg",SoundType.Scene);
