function maximiseOptions(e)
{	
	getMaximiseOptionsDiv().style.display = '';
	getMinimiseOptionsDiv().style.display = 'none';
}

function minimiseOptions(e)
{
	getMinimiseOptionsDiv().style.display = '';
	getMaximiseOptionsDiv().style.display = 'none';
}

function changeColorKeyDisplay(e)
{
    if (isColorKeyOpen())
    {
        closeColorKey(e);
    }
    else
    {
        openColorKey(e);
    }
}

function isColorKeyOpen()
{
    return getColorKeyWindowDiv().style.display == '';
}

function closeColorKey(e)
{
    getColorKeyWindowDiv().style.display = 'none';
}

function openColorKey(e)
{
    getColorKeyWindowDiv().style.display = '';
}

function changeCommentDisplay(commentId)
{
    var commentSpan = getDocumentObj(commentId);

    var commentSpanHidden = "commentText hideComment";
    var commentSpanVisible = "commentText";

    if (commentSpan.className == commentSpanHidden)
    {
        commentSpan.className = commentSpanVisible;
    }
    else
    {
        commentSpan.className = commentSpanHidden;
    }
}

function getMaximiseOptionsDiv()
{
	return getDocumentObj("OptionsMaximised");
}
	
function getMinimiseOptionsDiv()
{
	return getDocumentObj("OptionsMinimised");
}

function getColorKeyWindowDiv()
{
    return getDocumentObj("ColorKey");
}

function getTransOriginCheckbox()
{
    return getDocumentObj("transOrigin");
}

function getSourceCheckbox()
{
    return getDocumentObj("showSource");
}

function getTargetCheckbox()
{
    return getDocumentObj("showTarget");
}

function getShowTagsRadio()
{
    return getDocumentObj("showTags");
}

//function getShowAllContentRadio()
//{
//    return getDocumentObj("allContent");
//}

function getShowTextOnlyRadio()
{
    return getDocumentObj("textOnly");
}

function getCommentsCheckbox()
{
    return getDocumentObj("comments");
}

function applyChanges()
{
    applyCommentsSetting();
    applyTransOriginSetting();
    applyShowSourceTargetSetting();
    applyTagsSetting();
    
    minimiseOptions(null);
}

function applyCommentsSetting()
{
    if(getCommentsCheckbox().checked)
    {
        getStyleClass("hideComment").style.display = '';
    }
    else
    {
        getStyleClass("hideComment").style.display = 'none';
    }
}

function applyTransOriginSetting()
{
    if(getTransOriginCheckbox().checked)
    {
        getStyleClass("hideTransOrigin").style.color = "";
    }
    else
    {
        try
	    {
	        //Firefox/standards compatible
            getStyleClass("hideTransOrigin").style.color = "inherit";
	    }
	    catch(e)
	    {
	        //IE throws an exception on "inherit" value so use "transparent" instead
	        getStyleClass("hideTransOrigin").style.color = "transparent";
	    }
    }
}

function applyShowSourceTargetSetting()
{
    //show or hide the source
    if (getSourceCheckbox().checked)
    {
        getStyleClass("sourceCell").style.display = "";
        getStyleClass("targetCell").style.width = "50%";
        
    }
    else
    {
        getStyleClass("sourceCell").style.display = "none";
        getStyleClass("targetCell").style.width = "100%";
    }
    
    //show or hide the target
    if (getTargetCheckbox().checked)
    {
        getStyleClass("targetCell").style.display = "";
        getStyleClass("sourceCell").style.width = "50%";
    }
    else
    {
        getStyleClass("targetCell").style.display = "none";
        getStyleClass("sourceCell").style.width = "100%";
    }
}

function applyTagsSetting()
{
    if (getShowTagsRadio().checked)
    {
        getStyleClass("tag").style.display = "";
        getStyleClass("external").style.display = "none";
    }
    //else if (getShowAllContentRadio().checked)
    //{
    //    getStyleClass("tag").style.display = "";
    //    getStyleClass("external").style.display = "";
    //}
    else if (getShowTextOnlyRadio().checked)
    {
        getStyleClass("tag").style.display = "none";
        getStyleClass("external").style.display = "none";
    }    
}

function getDocumentObj(id)
{
	return document.getElementById(id);
}

function getStyleClass (className)
{
	for (var i = 0; i < document.styleSheets.length; i++)
	{
	    //check for browser compatibility
		if(document.styleSheets[i].rules)
		{
			for (var j = 0; j < document.styleSheets[i].rules.length; j++)
			{
				if (document.styleSheets[i].rules[j].selectorText == '.' + className)
				{
					return document.styleSheets[i].rules[j];
				}
			}
		}
		else if(document.styleSheets[i].cssRules)
		{
			for (var k = 0; k < document.styleSheets[i].cssRules.length; k++)
			{
				if (document.styleSheets[i].cssRules[k].selectorText == '.' + className)
				{
					return document.styleSheets[i].cssRules[k];
				}
			}
		}
	}
	
	return null;
}