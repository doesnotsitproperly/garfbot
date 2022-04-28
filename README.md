# GarfBot

A stupid Discord bot for my Discord server

The required `garf_data.json` file should look something like this:
```json
{
    "token": "ABC123",
    "path_to_ffmpeg": "path/to/ffmpeg_executable",
    "jokes": [
        "what's the deal with airline food?",
        "your mom!"
    ],
    "trigger_words": [
        "dang",
        "frick",
        "elon musk"
    ]
}
```
`path_to_ffmpeg` should include the file extension of the FFmpeg executable, which can can be downloaded from [the FFmpeg website](https://ffmpeg.org/)  
It can be left empty (`"path_to_ffmpeg": ""`) if you don't use the `play` command

Made with [Nextcord](https://github.com/nextcord/nextcord)
