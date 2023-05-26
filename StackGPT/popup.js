// adding a new bookmark row to the popup
const addNewBookmark = () => {};

const viewBookmarks = () => {};

const onPlay = e => {};

const onDelete = e => {};

const setBookmarkAttributes =  () => {};

document.addEventListener("DOMContentLoaded", () => {
    
try {


const container = document.getElementsByClassName("title")[0];
const checkbox = document.getElementById("myCheckbox");
const button = document.getElementById("button");
const textbox = document.getElementById("input");

container.innerHTML  = '<div class="title">stackGPT Version 1.0 </div>';
   
 getstatus(function (status) {
 if(status==null)
 {
     savestatus(false);
     status = false;
 }
  checkbox.checked = status;

  checkbox.addEventListener("click", function() {
   if (checkbox.checked) 
       savestatus(true);
   else 
       savestatus(false);
});

button.addEventListener("click", function() {
  
    if(textbox.value.lenght >= 10) 
    {
        alert("Key nicht lang genug :(");
        return;
    }
     chrome.storage.local.set({ "KEY": textbox.value }, function() {
          chrome.storage.local.get("KEY", function (result) {
               alert("Saved:         "+result.KEY);
            });
    });
  

});});

}
catch (error) { container.innerHTML  = '<div class="title"> '+error+'</div>';}

});


function getstatus(callback) {
    chrome.storage.local.get("Status", function (result) {
        callback(result.Status);
            });
}
function savestatus(boolToSave) {
    chrome.storage.local.set({ "Status": boolToSave }, function () {});
}