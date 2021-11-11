local discordia = require("discordia")
local client = discordia.Client()

trigger_words = {
    "bongo",
    "cum",
    "sex"
}

function trigger_word_in_message(check)
    for _, word in pairs(trigger_words) do
        if string.find(check, word) then return true end
    end
    return false
end

-- 
function get_table_length(t)
    local count = 0
    for _ in pairs(t) do count = count + 1 end
    return count
end

client:on("ready", function()
    print("Logged in as " .. client.user.username)
end)

client:on("messageCreate", function(message)
    msg = string.lower(message.content)

    -- React w/ :eyes: if someone mentions lasagna
    if string.find(msg, "lasagna") then
        message:addReaction("👀")
    end
    
    -- React w/ :rage: if someone mentions mondays
    if string.find(msg, "monday") then
        message:addReaction("😡")
    end
    
    -- Add a joke to "jokes.txt"
    if string.sub(msg, 1, string.len("garf add ")) == "garf add " then
        f = io.open("jokes.txt", "a")
        io.output(f)
        joke = string.sub(message.content, 10, string.len(message.content))
        io.write("\n" .. joke)
        f:close()
        message.channel:send("added joke: \"" .. joke .. "\"")
    
    -- List all jokes from "jokes.txt"
    elseif string.sub(msg, 1, string.len("garf jokes")) == "garf jokes" then
        f = io.open("jokes.txt", "r")
        io.input(f)
        jokes = {}
        for joke in io.lines() do table.insert(jokes, joke) end
        f:close()
        list = ""
        for _, joke in pairs(jokes) do list = list .. joke end
        message.channel:send(list)
    
    -- Say a joke if someone says a trigger word
    elseif trigger_word_in_message(msg) then
        f = io.open("jokes.txt", "r")
        io.input(f)
        jokes = {}
        for joke in io.lines() do table.insert(jokes, joke) end
        f:close()
        message.channel:send(jokes[math.random(get_table_length(jokes))])
    end
end)

-- Run the bot with token read from "token.txt"
f = io.open("token.txt", "r")
io.input(f)
client:run("Bot " .. io.read())
f:close()
