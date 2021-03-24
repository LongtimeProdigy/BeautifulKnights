using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class SoundManager : MonoSingleton<SoundManager> {

    AudioSource audiosourceBGM;
    AudioSource[] audiosourceSFX = new AudioSource[5];

    public AudioClip[] arrBGM = new AudioClip[1];
    public AudioClip[] arrSFX = new AudioClip[9];

    Dictionary<string, AudioClip> listBGM = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> listSFX = new Dictionary<string, AudioClip>();

	public void Initialize()
    {
        audiosourceBGM = transform.Find("BGM").GetComponent<AudioSource>();
        for(int i=0; i<audiosourceSFX.Length; i++)
        {
            audiosourceSFX[i] = transform.Find(string.Format("SFX{0}", i + 1)).GetComponent<AudioSource>();
        }

        foreach(AudioClip BGM in arrBGM)
        {
            listBGM.Add(BGM.name, BGM);
        }
        foreach (AudioClip SFX in arrSFX)
        {
            listSFX.Add(SFX.name, SFX);
        }

        audiosourceBGM.clip = listBGM["Effect_BGM_0001"];
        audiosourceBGM.Play();
    }

    // 사운드 재생(반복여부 포함)
    public void PlaySFX(string _name)
    {
        if (listSFX.ContainsKey(_name))
        {
            for (int i = 0; i < audiosourceSFX.Length; i++)
            {
                if (!audiosourceSFX[i].isPlaying)
                {
                    audiosourceSFX[i].clip = listSFX[_name];
                    audiosourceSFX[i].Play();
                    break;
                }
            }
        }
        else
        {
            Debug.Log(_name + " is not found");
        }
    }
    // 사운드 재생(반복여부, 볼륨 포함)
    public void PlaySFX(string _name, float _volum, bool islooping)
    {
        if (listSFX.ContainsKey(_name))
        {
            for (int i = 0; i < audiosourceSFX.Length; i++)
            {
                if (!audiosourceSFX[i].isPlaying)
                {
                    audiosourceSFX[i].clip = listSFX[_name];
                    audiosourceSFX[i].volume = _volum;
                    audiosourceSFX[i].Play();
                    break;
                }
            }
        }
        else
        {
            Debug.Log(_name + " is not found");
        }
    }
    // 사운드 한번 재생
    public void PlayOneShotSFX(string _name)
    {
        if (listSFX.ContainsKey(_name))
        {
            for (int i = 0; i < audiosourceSFX.Length; i++)
            {
                if (!audiosourceSFX[i].isPlaying)
                {
                    audiosourceSFX[i].PlayOneShot(listSFX[_name]);

                    break;
                }
            }
        }
        else
        {
            Debug.Log(_name + " is not found");
        }
    }
    // 사운드 한번 재생 및 볼륨 조절
    public void PlayOneShotSFX(string _name, float _volum)
    {
        if (listSFX.ContainsKey(_name))
        {
            for (int i = 0; i < audiosourceSFX.Length; i++)
            {
                if (!audiosourceSFX[i].isPlaying)
                {
                    audiosourceSFX[i].PlayOneShot(listSFX[_name], _volum);

                    break;
                }
            }
        }
        else
        {
            Debug.Log(_name + " is not found");
        }
    }

    public void StopSFX(string _name)
    {
        if (listSFX.ContainsKey(_name))
        {
            for (int i = 0; i < audiosourceSFX.Length; i++)
            {
                if (audiosourceSFX[i].isPlaying)
                {
                    if(audiosourceSFX[i].clip == listSFX[_name])
                    {
                        audiosourceSFX[i].Stop();
                        audiosourceSFX[i].clip = null;
                        audiosourceSFX[i].volume = 1f;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log(_name + " is not found");
        }
    }
}
