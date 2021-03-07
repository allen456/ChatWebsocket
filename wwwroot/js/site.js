function htmlEscape(str) 
{
    return str.toString()
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

function isJson(str) 
{
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}

function SendMenssahe(){
    if (socket && document.getElementById("sendMessage").value!= "") 
    {
        socket.send(JSON.stringify(
        {
            "From": document.getElementById("connIDLabel").innerHTML,
            "To": document.getElementById("recipient").value,
            "Message": document.getElementById("sendMessage").value
        }));
        AddSockComLog(false, "Client", "Server", htmlEscape(document.getElementById("sendMessage").value), "", "");
        document.getElementById("sendMessage").value = "";
    }
}

function ClientListClick(sagoters)
{
    PopulateClientList();
    var sagotsagots = document.getElementById(sagoters).getAttribute('data-anoba');
    if(sagotsagots == "Everybody")
    {
        document.getElementById("recipient").value = '';
        document.getElementById("sendButton").innerHTML = 'Send to Everyone';
    }
    else
    {
        document.getElementById("recipient").value = sagotsagots;
        document.getElementById("sendButton").innerHTML = 'Send to ' + sagotsagots;
    }
}
function PopulateClientList(){
    var xhrcliente = new XMLHttpRequest();
    xhrcliente.onreadystatechange = function()
    {
        if (xhrcliente.readyState === 4)
        {
            var returnclientlist = JSON.parse(xhrcliente.responseText);
            document.getElementById("clientelistodisplayo").innerHTML = '<a id="ClientListEverybody" href="#" onclick="ClientListClick(this.id);" class="list-group-item list-group-item-action bg-primary text-white" data-anoba="Everybody">Everybody</a>'
            returnclientlist.forEach(function(clientdata) 
            {
                var youdatabadge = '';
                if(clientdata.chatClientName == document.getElementById("connIDLabel").innerHTML)
                {
                    youdatabadge = ' <span class="badge bg-danger">You</span>';
                }
                document.getElementById("clientelistodisplayo").innerHTML += '<a id="ClientList' + clientdata.chatClientId + '" href="#" onclick="ClientListClick(this.id);" class="list-group-item list-group-item-action" data-anoba="'+ clientdata.chatClientName + '">'+ clientdata.chatClientName + youdatabadge + '</a>';
            });
        }
    };
    xhrcliente.open('GET', '/Chat/GetClientList');
    xhrcliente.send()
}

function AddSockComLog(displaydata, displayfrom, displayto, displaymessage, displaydate, everyone)
{
    if(displaydata)
    {
        var dispdate = displaydate;
        if(everyone)
        {
            displayto = ' <span class="badge bg-primary">Everyone</span>';
        }
        else
        {
            if(displayto == document.getElementById("connIDLabel").innerHTML)
            {
                displayto = ' <span class="badge bg-danger">' + displayto + ' (You)</span>';
            }
            else
            {
                displayto = ' <span class="badge bg-secondary">' + displayto + '</span>';
            }
        }
        if(document.getElementById("connIDLabel").innerHTML == displayfrom)
        {
            if(displayfrom == document.getElementById("connIDLabel").innerHTML)
            {
                displayfrom = '<span class="badge bg-danger">'+ displayfrom +' (You)</span>';
            }
            else
            {
                displayfrom = '<span class="badge bg-secondary">'+ displayfrom +'</span>';
            }
            document.getElementById("communicationdisplay").insertAdjacentHTML("afterbegin", '<div class="list-group-item text-end"><div class="d-flex w-100 justify-content-between"><small>' + dispdate + '</small><h2 class="mb-1">' + displaymessage + '</h2></div><p class="mb-1 text-end">From: ' + displayfrom + ' To: ' + displayto + '</p></div>');
        }
        else
        {
            if(displayfrom == document.getElementById("connIDLabel").innerHTML)
            {
                displayfrom = '<span class="badge bg-danger">'+ displayfrom +' (You)</span>';
            }
            else
            {
                displayfrom = '<span class="badge bg-secondary">'+ displayfrom +'</span>';
            }
            document.getElementById("communicationdisplay").insertAdjacentHTML("afterbegin", '<div class="list-group-item"><div class="d-flex w-100 justify-content-between"><h2 class="mb-1">' + displaymessage + '</h2><small>' + dispdate + '</small></div><p class="mb-1">From: ' + displayfrom + ' To: ' + displayto + '</p></div>');
        }
    }
    else
    {

    }
}

function connekSucket(socketurl){
    socket = new WebSocket(socketurl);
    socket.onopen = function (event) 
    {
        document.getElementById("stateLabel").innerHTML = "Connected";
        document.getElementById("sendMessage").disabled = false;
        document.getElementById("sendButton").disabled = false;
        AddSockComLog(false, 'Server', 'Client', 'Connection opened', "", "");
        Swal.close()
        PopulateClientList();
    };
    socket.onclose = function (event) 
    {
        document.getElementById("stateLabel").innerHTML = "Closed";
        document.getElementById("connIDLabel").innerHTML = "Closed"
        document.getElementById("sendMessage").disabled = true;
        document.getElementById("sendButton").disabled = true;
        AddSockComLog(false, 'Server', 'Client', 'Connection closed ' + htmlEscape(event.code), "", "");
        let timerInterval
        Swal.fire({
            title: 'I cant find the server',
            html: 'Trying to find again in <b></b> milliseconds.',
            timer: 30000,
            timerProgressBar: true,
            allowOutsideClick: false,
            didOpen: () => 
            {
                Swal.showLoading();
                timerInterval = setInterval(() => 
                {
                    const content = Swal.getContent()
                    if (content) {
                        const b = content.querySelector('b')
                        if (b) {
                            b.textContent = Swal.getTimerLeft()
                        }
                    }
                }, 100);
            },
            willClose: () => { clearInterval(timerInterval); }
        })
        .then((result) => 
        {
            if (result.dismiss === Swal.DismissReason.timer) 
            {
                connekSucket(document.getElementById("connectionUrl").innerHTML);
            }
        });
    };
    socket.onmessage = function (event) 
    {
        AddSockComLog(false, 'Server', 'Client', htmlEscape(event.data), "", "");
        PopulateClientList();
        if(isJson(event.data))
        {
            var objdata = JSON.parse(event.data);  
            AddSockComLog(true, objdata.Galing, objdata.Papunta, htmlEscape(objdata.Laman), objdata.Araw, objdata.Lahat);
        }else{
            document.getElementById("connIDLabel").innerHTML = event.data;
        }
    };
}