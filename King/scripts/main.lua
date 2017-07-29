local pauls_cows = {
    txt='Your majesty! My cows are sick.',
    choices={
        'Go away, you filthy peasant!',
        'I can\'t help you. Here take some money.',
    }
}

function pauls_sick_cows(num)
    if (num == 1) then
        Log('Paul the peasant is sad.')
        --- make paul sad
    else
        Log('Paul the peasant is getting drunk tonight.')
        --- make paul a little less sad
    end
end

SetNewChoice(pauls_cows.txt, pauls_sick_cows, pauls_cows.choices)
