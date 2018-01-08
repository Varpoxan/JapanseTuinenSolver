var Ids = [];
var MatchType;
var RankType;
var AllPlayers = false;
var BallsOffTable = [];

var TeamBalls = {
    TeamOne: [],
    TeamTwo: [],
    Fails: []
};

var GameHasWinner = false;

function ResetParameters(rankType, matchType) {
    MatchType = matchType;
    RankType = rankType;
    Ids = [];
    TeamBalls = {
        TeamOne: [],
        TeamTwo: [],
        Fails: []
    };
    BallsOffTable = {
        TeamOne: [],
        TeamTwo: []
    };
    AllPlayers = false;
    GameHasWinner = false;
}

$(document).off('click', '.SwapPlayers').on('click', '.SwapPlayers', function () {

    if ($(this).hasClass('disabled')) return;

    if ($('.PlayerTwo .PlayerSlot').data('player-id') !== 0 &&
        $('.PlayerOne .PlayerSlot').data('player-id') !== 0) {
        Ids.pop();
        Ids.pop();
        Ids.push($('.PlayerTwo .PlayerSlot').data('player-id'));
        Ids.push($('.PlayerOne .PlayerSlot').data('player-id'));

        FillPlayerContainer();
        FillDuelStats();
    }
});

$(document).off('click', '.MatchSetup .PlayerCard').on('click', '.MatchSetup .PlayerCard', function () {

    var clickedPlayerId = $(this).data('player-id');
    var PlayerSlotOne = $('.PlayerTwo .PlayerSlot').data('player-id');
    var PlayerSlotTwo = $('.PlayerOne .PlayerSlot').data('player-id');

    $('.PlayMatch').prop('disabled', true);
    if (jQuery.inArray(clickedPlayerId, Ids) === -1 && Ids.length < 2) {
        Ids.push(clickedPlayerId);
    }

    //Only push the PlayerContainer if there are slots available
    if ((PlayerSlotOne === 0 || PlayerSlotTwo === 0) && !$(this).hasClass('selected')) {
        $(this).addClass('selected');
        //$(this).toggleClass('disabled');
        FillPlayerContainer();
    }

    //If the clickedplayer is in one of the slots, it's possible to remove
    if (PlayerSlotOne === clickedPlayerId || PlayerSlotTwo === clickedPlayerId && $(this).hasClass('selected')) {
        Ids.splice($.inArray(clickedPlayerId, Ids), 1);
        $(this).removeClass('selected');
        FillPlayerContainer();
    }
    //FillPlayerContainer();
    FillDuelStats();
});

function FillPlayerContainer() {

    var data = {
        PlayerIds: Ids,
        GameType: MatchType
    };

    $.ajax({
        url: "Play/MatchPrognosis",
        data: data,
        type: 'post',
        cache: false
    }).done(function (html) {
        $('.PlayerContainer').html(html);

        if ($('.PlayerTwo .PlayerSlot').data('player-id') !== 0 &&
            $('.PlayerOne .PlayerSlot').data('player-id') !== 0) {
            //Disable the rest of the PlayerCards when both slots are filled
            $('.PlayerCard').each(function () {
                $(this).addClass('disabled');
            });
            //Enable the PlayerCards that are in the match, to be able to remove them
            $.each(Ids, function (index, value) {
                $('#Player_' + value).removeClass('disabled');
            });
            $('.PlayMatch').prop('disabled', false);
            $('.PlayMatch').addClass('ReadyToPlay');
        }
        else {
            $('.PlayerCard').each(function () {
                $(this).removeClass('disabled');
                $.each(Ids, function (index, value) {
                    $('#Player_' + value).addClass('disabled');
                });
            });
            $('.PlayMatch').removeClass('ReadyToPlay');
        }
    });
}

function FillDuelStats() {
    if (Ids.length < 2) {
        $('.SeasonStats').remove();
        return;
    }

    var data = {
        PlayerIds: Ids,
        GameType: MatchType,
        ImportanceType: RankType
    };

    $.ajax({
        url: "Play/DuelStats",
        data: data,
        type: 'post',
        cache: false
    }).done(function (html) {

        $('.DuelStats').html(html);

    });
}

$(document).off('click', '.PlayMatch').on('click', '.PlayMatch', function () {

    $('.PlayMatch').addClass('hidden');
    $('.ShowMatchSetup').removeClass('hidden').removeAttr('disabled');
    $('.SaveMatch').removeClass('hidden');
    $('.MatchSetup').addClass('Collapse');
    $('.AllPlayers').addClass('Collapse');
    $('.MatchToolBar .selectable-matchtypes').addClass('Collapse');
    $('.whack-ball-off-table').removeClass('hidden');

    ToggleMatchType(MatchType);

});

function ToggleMatchType(matchType) {
    if (matchType === '8-Ball') {
        Enable8BallFunctions();
        Disable9BallFunctions();
        Disable15BallFunctions();
    }
    if (matchType === '9-Ball') {
        Disable8BallFunctions();
        Enable9BallFunctions();
        Disable15BallFunctions();
    }
    if (matchType === '15-Ball') {
        Disable8BallFunctions();
        Disable9BallFunctions();
        Enable15BallFunctions();
    }
}

function Enable8BallFunctions() {
    //Show Solid/Striped balls on the screen
    if ($('.MatchSetup').hasClass('Collapse')) {
        $('.eight-balltype-divider').removeClass('hidden');
    }
}

function Disable8BallFunctions() {
    //Hide Solid/Striped balls on the screen
    $('.eight-balltype-divider').addClass('hidden');
}

function Enable9BallFunctions() {
    //Show Solid/Striped balls on the screen
    $('.SwapPlayers').addClass('disabled');
}

function Disable9BallFunctions() {
    //Hide Solid/Striped balls on the screen
    $('.SwapPlayers').removeClass('disabled');
}

function Enable15BallFunctions() {

    if ($('.MatchSetup').hasClass('Collapse')) {
        $.ajax({
            url: "Play/FifteenBallViewModel",
            type: 'post',
            cache: false
        }).done(function (html) {
            $.when($('.MatchSetup').after(html)).done(function () {

                $('.DuelStats').addClass('Collapse');
                AddHtmlPointsToWin(Ids[0], 61);
                AddHtmlPointsToWin(Ids[1], 61);
                $('.PlayerSlot .Avatar').addClass('FifteenBall');
                $('.name-and-birth').addClass('Collapse');
                $('.SwapPlayers').addClass('disabled');
            });
        });
    }
}

function Disable15BallFunctions() {
    $('.MatchSetup .fifteen-ball-game').remove();
    $('.DuelStats').removeClass('Collapse');
    $('.PlayerSlot .Avatar').removeClass('FifteenBall');
    $('.name-and-birth').removeClass('Collapse');
    $('.SwapPlayers').removeClass('disabled');
    $('.match-score').empty();
    $('.play-fifteenball').remove();
    $('.points-to-win').remove();
    TeamBalls = {
        TeamOne: [],
        TeamTwo: [],
        Fails: []
    };
}

$(document).off('click', '.CurrentPlayerMatches .duel-stats-toggle-button').on('click', '.duel-stats-toggle-button', function () {

    var clickedId = $(this).attr('id');

    $('.duel-stats-toggle-button').each(function () {
        $(this).removeClass('active');
    });
    $(this).addClass('active');

    $('.DuelInfoStats').each(function () {
        $(this).addClass('hidden');
    });

    //console.log(clickedId + '-info');
    $('.' + clickedId + '-info').removeClass('hidden');

    $('.SeasonStats .plays').each(function () {
        $(this).addClass('hidden');
    });
    $('.' + clickedId + '-plays').removeClass('hidden');
});

$(document).off('click', '.ShowMatchSetup').on('click', '.ShowMatchSetup', function () {

    $('.MatchSetup').removeClass('Collapse');
    $('.DuelStats').removeClass('Collapse');
    $('.AllPlayers').removeClass('Collapse');
    $('.MatchToolBar .selectable-matchtypes').removeClass('Collapse');
    $('.PlayMatch').removeClass('hidden');
    $('.SaveMatch').addClass('hidden');
    $('.ShowMatchSetup').addClass('hidden');
    $('.whack-ball-off-table').addClass('hidden');
    Disable8BallFunctions();
    Disable9BallFunctions();
    Disable15BallFunctions();
});

$(document).off('click', '.SaveMatch').on('click', '.SaveMatch', function () {

    $(this).prop('disabled', true);
    $('.Match').spinner({ inheritBackground: false, text: 'Saving match...' });
    var WinnerId = $('.PlayerSlot.Winner').data('player-id');

    var MatchPlayers = {};

    $(Ids).each(function (i, e) {

        MatchPlayers[i] = {
            PlayerId: e,
            IsWinner: WinnerId === e,
            BallType: { Name: $('#PlayerSlot_' + e).data('sphere-type') }
        };
    });

    if (MatchType === '15-Ball') {
        $(Ids).each(function (i) {
            MatchPlayers[i].Balls = {};

            var PlayerBalls;
            if (i === 0) {
                PlayerBalls = TeamBalls.TeamOne;
            }
            else {
                PlayerBalls = TeamBalls.TeamTwo;
            }

            $(PlayerBalls).each(function (j, g) {
                MatchPlayers[i].Balls[j] = {
                    Points: g
                };
            });
        });
    }

    var data = {
        MatchPlayers: MatchPlayers,
        MatchType: { Name: MatchType },
        RatingType: { Name: RankType },
        WhackBallsOffTable: BallsOffTable
    };

    //debugger;
    console.log(data);

    $.ajax({
        url: "Play/SaveMatch",
        data: data,
        type: 'post',
        cache: false
    }).done(function (html) {

        $('.Match').spinner().hide();
        //$('.Curve').show();
        $('.Content').html(html);

        Disable15BallFunctions();
        Ids = [];
    });

});

$(document).off('click', '.MatchToolBar .ball').on('click', '.MatchToolBar .ball', function () {

    var newGameType = $(this).attr('id').replace('sub-menu-', '');

    $('.ball').each(function () {
        $(this).removeClass('active');
    });

    $(this).addClass('active');

    if (newGameType === MatchType) { return; }
    MatchType = newGameType;

    $('.PlayerCard .Rating').each(function () {
        var rating = $(this).data(MatchType.toLowerCase() + '-rating');
        $(this).text('ELO ' + rating);
    });

    $(".PlayerCardCollection .PlayerCard").sort(sort_li) // sort elements
        .appendTo('.PlayerCardCollection'); // append again to the list
    // sort function callback
    function sort_li(a, b) {
        return ($(b).find('.Rating').data(MatchType.toLowerCase() + '-rating')) > ($(a).find('.Rating').data(MatchType.toLowerCase() + '-rating')) ? 1 : -1;
    }

    $.when(FillPlayerContainer()).done(function () {
        if (Ids.length === 2) {
            setTimeout(FillDuelStats(), 500);
            ToggleMatchType(MatchType);
        }
    });
});


$(document).off('click', '.AvatarWrapper').on('click', '.AvatarWrapper', function () {

    if (MatchType !== '15-Ball') {
        $('.PlayerSlot').each(function () {
            $(this).removeClass('Winner');
        });

        if (Ids.length === 2 && $('.MatchSetup').hasClass('Collapse')) {
            $(this).closest('.PlayerSlot').addClass('Winner');
            $('.SaveMatch').prop('disabled', false);
        }
    }
});

$(document).off('click', '.whack-ball-off-table').on('click', '.whack-ball-off-table', function () {
    var $cannon = $(this);
    $cannon.addClass('active');

    $('.PlayerSlot').on('click', function () {
        var $playerSlot = $(this);
        if ($playerSlot.closest('.Player').hasClass('PlayerOne'))
            $cannon.addClass('turn-around');
        else
            $cannon.removeClass('turn-around');
    });

    ToggleWhackBallOffTable();
});

$(document).off('click', '.WhackBallOffTableCancel').on('click', '.WhackBallOffTableCancel', function () {
    $(this).removeClass('active');

    ToggleWhackBallOffTable();
    $('.PlayerSlot').removeClass('Winner');
});

function ToggleWhackBallOffTable() {
    $('.PlayerSlotStats').toggleClass('Collapse');
    $('.GameTypeContent').toggleClass('Collapse');
    $('.MatchFooter').toggleClass('Collapse');
    $('.whack-ball-off-table-player-selection').toggleClass('Collapse');
};

$(document).off('click', '.WhackBallOffTableSave').on('click', '.WhackBallOffTableSave', function () {
    $cannon = $('.whack-ball-off-table');
    $cannon.addClass('fire');

    var $ball = $('<div class="cannon-ball ball _8-Ball">');
    $cannon.append($ball);

    var selectedPlayerId = $('.Winner').data('player-id');

    if (selectedPlayerId !== 0 && selectedPlayerId !== null && selectedPlayerId !== undefined) {

        //console.log(BallsOffTable);
        //console.log(BallsOffTable.Player.Id == selectedPlayerId);
        var winnerParent = $('.Winner').parent()[0];
        if (winnerParent.id.indexOf('1') !== -1) {
            BallsOffTable.TeamOne = BallsOffTable.TeamOne += 1;
        }
        else {
            BallsOffTable.TeamTwo = BallsOffTable.TeamTwo += 1;
        }

        //$.ajax({
        //    url: "Play/PointsToWin",
        //    data: {
        //        pointsToWin: pointsToWin,
        //        playerId: playerId
        //    },
        //    type: 'post',
        //    cache: false
        //}).done(function (html) {

        //    //$('#PlayerSlot_' + playerId).find('.PlayerSlot').prepend(html);
        //});

        //console.log(selectedPlayerId);
        console.log(BallsOffTable);
        //console.log(BallsOffTable.Player);

        ToggleWhackBallOffTable();
        $('.PlayerSlot').removeClass('Winner');
        $('.SaveMatch').prop('disabled', true);
    }
});

$(document).off('click', '.RankedGameCheckbox').on('click', '.RankedGameCheckbox', function () {

    if (RankType === "Ranked") {
        RankType = "NonRanked";
    }
    else {
        RankType = "Ranked";
    }
});

$(document).off('click', '.AllPlayersCheckbox').on('click', '.AllPlayersCheckbox', function () {

    if (AllPlayers === false) {
        $('.PlayerCard.Inactive').each(function () {
            $(this).removeClass('Inactive');
            $(this).addClass('PotentialInactive');
        });
    }
    else {
        $('.PlayerCard.PotentialInactive').each(function () {
            $(this).removeClass('PotentialInactive');
            $(this).addClass('Inactive');
        });
    }

    AllPlayers = !AllPlayers;

});

$(document).off('click', '.post-match-done').on('click', '.post-match-done', function () {
    location.reload();
});

$(document).off('click', '.collapse-triangle').on('click', '.collapse-triangle', function () {

    $('.post-match-screen .collapsible').toggleClass('Collapse');
    //$('.post-match-ratings').toggleClass('Collapse');

});

$(document).off('click', '.play-fifteenball .team-ball').on('click', '.play-fifteenball .team-ball', function () {

    var ballPoints = $(this).data('points');
    var classList = $(this).attr('class').split(/\s+/);

    var playerIndex = 2;
    if (($(this).attr('id').indexOf('team-one') !== -1)) {
        playerIndex = 0;
    }
    else if (($(this).attr('id').indexOf('team-two') !== -1)) {
        playerIndex = 1;
    }

    if (!$(this).hasClass('disabled')) {
        if (GameHasWinner === true) { return; }

        Update15BallScore($(this), playerIndex);

        for (var i = 0; i < classList.length; i++) {
            if (classList[i].indexOf('-Ball') !== -1) {
                $('.' + classList[i]).addClass('disabled');
                if (playerIndex === 0) {
                    //debugger;
                    $('#team-two-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                    $('#fail-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                }
                if (playerIndex === 1) {
                    $('#team-one-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                    $('#fail-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                }
                if (playerIndex === 2) {
                    $('#team-one-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                    $('#team-two-' + $(this).data('ball-id') + '-Ball').addClass('permanently-disabled');
                }
            }
        }
    }
    else { //If the ball is disabled, we can refund the ball for the relevant player 
        TryRefunBallToTeam(ballPoints, TeamBalls.TeamOne, $(this), 'team-one');
        TryRefunBallToTeam(ballPoints, TeamBalls.TeamTwo, $(this), 'team-two');
        TryRefunBallToTeam(ballPoints, TeamBalls.Fails, $(this), 'fail');
        if (!$(this).hasClass('permanently-disabled'))
            $('._' + $(this).data('ball-id') + '-Ball').removeClass('permanently-disabled');
    }
    //debugger;
    //If the ball has failed, we need to update both player labels
    if (playerIndex === 2) {
        if (GetCurrentScoreFromArray(0) > GetCurrentScoreFromArray(1)) {
            UpdatePlayerScoreLabel(1);
            UpdatePlayerScoreLabel(0);
        }
        else { // This was needed because the order made the save button enabled and directly disabled again, if the losing player was checked last..!
            UpdatePlayerScoreLabel(0);
            UpdatePlayerScoreLabel(1);
        }
    }
    else {
        UpdatePlayerScoreLabel(playerIndex);
    }
});

Array.prototype.remove = function (el) {
    return this.splice(this.indexOf(el), 1);
};

function TryRefunBallToTeam(ballPoints, BallsOfTeam, clickedObj, teamString) {
    var classList = $(clickedObj).attr('class').split(/\s+/);
    var ballIndex = BallsOfTeam.indexOf(ballPoints);

    if (ballIndex !== -1 && $(clickedObj).attr('id').indexOf(teamString) !== -1) {
        BallsOfTeam.remove(ballPoints);
        for (var i = 0; i < classList.length; i++) {
            if (classList[i].indexOf('-Ball') !== -1) {
                $('.' + classList[i]).removeClass('disabled');
            }
        }
    }
}

function UpdatePlayerScoreLabel(playerIndex) {
    var newScore = GetCurrentScoreFromArray(playerIndex);
    if (newScore === 0) {
        $('#PlayerSlot_' + Ids[playerIndex]).find('.match-score').empty();
    }
    else {
        $('#PlayerSlot_' + Ids[playerIndex]).find('.match-score').text(newScore);
    }

    var failedPoints = GetCurrentScoreFromArray(2);
    var playerId = Ids[playerIndex];
    var leftOverScore = (120 - failedPoints);// - totalPuttedScore;
    if ((leftOverScore) % 2 === 0) {
        UpdateHtmlPointsToWin(playerId, ((leftOverScore) / 2) - (newScore - 1));
    }
    else {
        UpdateHtmlPointsToWin(playerId, ((leftOverScore + 1) / 2) - newScore);
    }


}

function Update15BallScore(obj, playerIndex) {
    var ballPoints = $(obj).data('points');

    if (playerIndex === 0) {
        TeamBalls.TeamOne.push(ballPoints);
    }
    else if (playerIndex === 1) {
        TeamBalls.TeamTwo.push(ballPoints);
    }
    else if (playerIndex === 2) {
        TeamBalls.Fails.push(ballPoints);
    }
}

function GetCurrentScoreFromArray(playerIndex) {
    var cScore = 0;

    if (playerIndex === 0) {
        for (var i = 0; i < TeamBalls.TeamOne.length; i++) {
            cScore += TeamBalls.TeamOne[i];
        }

    }
    else if (playerIndex === 1) {
        for (var j = 0; j < TeamBalls.TeamTwo.length; j++) {
            cScore += TeamBalls.TeamTwo[j] << 0;
        }

    }
    else if (playerIndex === 2) {
        for (var k = 0; k < TeamBalls.Fails.length; k++) {
            cScore += TeamBalls.Fails[k] << 0;
        }
    }

    return cScore;
}

function AddHtmlPointsToWin(playerId, pointsToWin) {

    $.ajax({
        url: "Play/PointsToWin",
        data: {
            pointsToWin: pointsToWin,
            playerId: playerId
        },
        type: 'post',
        cache: false
    }).done(function (html) {

        $('#PlayerSlot_' + playerId).find('.PlayerSlot').prepend(html);
    });

    //return
    //'<div class="points-to-win" id="points-to-win'+ playerId +'">' +
    //    '<div>TO WIN</div>'+
    //    '<div class="points-container">'+
    //        '<div class="point-digit"></div>'+
    //    '</div>'+
    //'</div>';
}

function UpdateHtmlPointsToWin(playerId, pointsToWin) {
    $.ajax({
        url: "Play/PointsToWin",
        data: {
            pointsToWin: pointsToWin,
            playerId: playerId
        },
        type: 'post',
        cache: false
    }).done(function (html) {

        $('#PlayerSlot_' + playerId).find('.points-to-win').replaceWith(html);

        //console.log(pointsToWin);
        var amountOfPotted = parseInt(TeamBalls.TeamOne.length) + parseInt(TeamBalls.TeamTwo.length) + parseInt(TeamBalls.Fails.length);
        //console.log('amount of scored balls: ' + amountOfPotted);
        if (parseInt(pointsToWin) <= 0 || amountOfPotted === 15) {
            //console.log('we gaan opslaan enablen!');
            $('.SaveMatch').prop('disabled', false);
            GameHasWinner = true;
        }
        else {
            $('.SaveMatch').prop('disabled', true);
            GameHasWinner = false;
        }
    });
}

$(document).off('click', '.toggle-season-all-time').on('click', '.toggle-season-all-time', function () {
    $('.player-rank-divider .season-ranking').toggleClass('hidden');
    $('.player-rank-divider .all-time-ranking').toggleClass('hidden');
});