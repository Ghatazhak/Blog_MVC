
let index = 0;



function AddTag() {
    // get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");

    // Lets use new search function
    let searchResult = Search(tagEntry.value);
    if (searchResult != null) {
        //Trigger my sweet alert for whatever condition
        swalWithDarkButton.fire({
            html:`<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        });

    } else {
       // Create a new select option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }

    // clear out the TagEntry control
    tagEntry.value = "";


}


function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>CHOOSE A TAG BEFORE DELETING</span>`
        });
        return true;
    }
    

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        } else {
            tagCount = 0;
        }
        index--;
    }

}



$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})


// Look for the tagValues variable to see if it has data

if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        //load or replace the options that we have
        ReplaceTag(tagArray[loop],loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}

// The search function will detect either an empty or a duplicate Tag
// And return an error string if an error is detected

function Search(str) {
    if (str == "") {
        return "Empty Tags are not permitted";
    }
    var tagsElement = document.getElementById("TagList");

    if (tagsElement) {
        let options = tagsElement.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {
                return `The Tag #${str} was detected as a duplicate and not permitted`;
            }
        }
    } 

}

const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm btn-outline-dark'
    },
    imageUrl: '/imgs/error.webp',
    timer: 3000,
    buttonsStyling: false
})