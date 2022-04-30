# GarfBot

A stupid Discord bot for my Discord server  
Made with [Nextcord](https://github.com/nextcord/nextcord)

The required `garf_data.json` file should look something like this:
```jsonc
{
    "token": "ABC123",
    "path_to_ffmpeg": "path/to/ffmpeg", // On Windows: "path\\to\\ffmpeg.exe"
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
Download info for FFmpeg can be found at [the FFmpeg website](https://ffmpeg.org/)  
`path_to_ffmpeg` can be left empty (`"path_to_ffmpeg": ""`) if you don't use the `play` command
