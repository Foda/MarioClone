using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    static class AudioManager
    {
        static List<SoundEffect> soundSFX = new List<SoundEffect>();

        public static void LoadContent(ContentManager content)
        {
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\deployitem.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\jump.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\kick.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\powerup.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\powerdown.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\stomp.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\shell_stop.wav")));
            soundSFX.Add(content.Load<SoundEffect>(System.IO.Path.GetFullPath("Sfx\\shell_rico.wav")));
        }

        public static void PlaySfx(string snd)
        {
            foreach (SoundEffect sfx in soundSFX)
            {
                if (snd == sfx.Name)
                    sfx.Play();
            }
        }
    }
}
