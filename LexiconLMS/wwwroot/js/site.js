
// Update variable which holds the current form-data
$(function () {
    var update = function () {
        $('#serializearray').text(
            JSON.stringify($(".activityForm").serializeArray())
        );
        // Maybe not do array but string instead?
    };
    update();
    $(".activityForm").change(update);
})



let remove = document.getElementsByClassName("remove");         // This classname is "activated" when one clicks addToList()

// List to hold all activities from the form data
let allActivities = [];

function addToList() {

    //let inputValue = document.getElementById("myInput").value;      // Hämta värde från inputfält.
    //let inputValue = document.getElementById("serializearray").value;      
    //let inputValue = $('#serializearray').text(JSON.stringify($(".activityForm").serializeArray()));      
    //let inputValue = $(".activityForm");
    //let inputValue = document.getElementById("serializearray").innerHTML;

    let obj = JSON.parse(document.getElementById("serializearray").innerHTML) // Serialisera 
    
    let inputValue = obj[0].value + " / " + obj[1].value + " / " + obj[3].value + " / " + obj[4].value;
    

    if (inputValue != "") {
        allActivities.push(obj);
    }

    //console.log(allActivities);
    

    

    if (inputValue === '') {                                        // Kolla om någon input skrivits in.
        alert("Tomt inputfält!");
    } else {
        let li = document.createElement("LI");                        // Skapa ny <li>-nod.
        let elementText = document.createTextNode(inputValue);        // Tillsätt inputvärde till en ny text-nod.
        li.appendChild(elementText);                                  // Lägg till text/barn-nod till nya <li>-noden. 
        let xButton = document.createElement("button");                 // Skapa x-knapp.
        let x = document.createTextNode("\u00D7");                      // Skapa variabel med x-symbol.
        xButton.className = "remove";                                   // Ge x-knappen ett klassnamn.
        xButton.appendChild(x);                                         // Lägg till x-symbolen till x-knappen.
        li.appendChild(xButton);                                        // Lägg till x-knappnoden till <li>
        document.getElementById("theList").appendChild(li);           // Lägg till listpunkt till listan i sig.
    }
                     
    //document.getElementById("serializearray").value = "";                  
    document.getElementById("serializearray").innerHTML = "";         // Nollställ inputvärde.         

    for (i = 0; i < remove.length; i++) {                           // Detta lägger till ta-bort-funktionen
        remove[i].onclick = function () {
            let parent = this.parentElement;
            parent.style.display = "none";
            for (var i = 0; i < allActivities.length; i++) {
                if (allActivities[i] === obj) {
                    allActivities.splice(i, 1);                 // Delete current object from list when the x-button is clicked.
                } 
            }
        }
    }
    
    
}




function sendJson() {

    var things = JSON.stringify({ 'allActivities': allActivities }); // Testing to preprocess array before sending

    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: 'POST',
        url: '/Modules/Create',  // If this does not work a a second arg to the action, create a new action that returns the stuff?
        data: things,
        success: function () {
            $('#result').html('"sendJson()" successfully called.');
        },
        failure: function (response) {
            $('#result').html(response);
        }
    }); 
}


