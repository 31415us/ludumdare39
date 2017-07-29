

text = "This gui sucks!"

function callback(num)
    if (num == 1) then
        text = "This gui sucks!"
    else
        text = text .. "!"
    end
    SetNewChoice(text, callback, {"reset", "add '!'"})
    Log(num)
end

SetNewChoice(text, callback, {"reset", "add '!'"})