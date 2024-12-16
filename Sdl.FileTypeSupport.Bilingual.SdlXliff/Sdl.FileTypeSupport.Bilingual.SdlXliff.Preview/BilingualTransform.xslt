<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
	xmlns:resources="urn:resources">

	<xsl:output method="html" encoding="utf-8" indent="no" doctype-system="http://www.w3.org/TR/html4/strict.dtd" doctype-public="-//W3C//DTD HTML 4.01//EN"/>

	<xsl:param name="css"></xsl:param>
	<xsl:param name="js"></xsl:param>
	
	<xsl:template match="/">
		<html>
			<head>
				<style>
					<xsl:value-of select="$css"/>
				</style>
				<script language="JavaScript">
					<xsl:value-of select="$js"/>
				</script>
			</head>
			<body>
				<xsl:apply-templates select="document/file"/>

				<div id="OptionsMinimised" class="optionsWindow optionsWindowPrint">
					<div class="optionsTitle">
						<span style="cursor: pointer; cursor: hand;" onclick="maximiseOptions(event);"><xsl:value-of select="resources:GetString('Preview_Options_Name')"/> | +</span>
					</div>
				</div>

				<div id="OptionsMaximised" class="optionsWindow optionsWindowPrint" style="display: none;" >
					<div class="optionsTitle" style="border-bottom: 1px solid black;">
						<span style="cursor: pointer; cursor: hand;" onclick="minimiseOptions(event);"><xsl:value-of select="resources:GetString('Preview_Options_Name')"/> | -</span>
					</div>
					<table>
            <tr>
              <td>
                <input id="showSource" type="checkbox" checked="true"/>
              </td>
              <td>
                <xsl:value-of select="resources:GetString('Preview_ShowSourceOption_Name')"/>
              </td>
            </tr>
            <tr>
              <td>
                <input id="showTarget" type="checkbox" checked="true"/>
              </td>
              <td>
                <xsl:value-of select="resources:GetString('Preview_ShowTargetOption_Name')"/>
              </td>
            </tr>
						<tr>
							<td>
								<input id="transOrigin" type="checkbox" checked="true"/>
							</td>
							<td>
								<xsl:value-of select="resources:GetString('Preview_TransOriginOption_Name')"/>
							</td>
						</tr>
						<tr>
							<td>
								<input id="comments" type="checkbox"/>
							</td>
							<td>
								<xsl:value-of select="resources:GetString('Preview_CommentsOption_Name')"/>
							</td>
						</tr>
            <tr>
              <td>
                <input id="textOnly" type="radio" name="tagSettings" checked="true"/>
              </td>
              <td>
                <xsl:value-of select="resources:GetString('Preview_NoTagsOption_Name')"/>
              </td>
            </tr>
            <tr>
              <td>
                <input id="showTags" type="radio" name="tagSettings"/>
              </td>
              <td>
                <xsl:value-of select="resources:GetString('Preview_ReducedTagsOption_Name')"/>
              </td>
            </tr>
            <!-- Not much point to this so comment out for now
            <tr>
              <td>
                <input id="allContent" type="radio" name="tagSettings"/>
              </td>
              <td>
                <xsl:value-of select="resources:GetString('Preview_AllTagsOption_Name')"/>
              </td>
            </tr> -->
						<tr>
							<td colspan="2">
								<xsl:variable name="applyChanges">
									<xsl:value-of select="resources:GetString('Preview_ApplyChangesButton_Name')"/>
								</xsl:variable>
								<input type="button" class="button" value="{$applyChanges}" onclick="applyChanges()" />
							</td>
						</tr>
            <tr>
              <td colspan="2">
                <span class="hyperlink" onclick="changeColorKeyDisplay(event);">
                  <xsl:value-of select="resources:GetString('Preview_ShowColorKeyLink_Name')"/>
                </span>
              </td>
            </tr>
					</table>
        <div id="ColorKey" class="optionsWindow optionsWindowPrint" style="display: none;">
          <div class="optionsTitle" style="border-bottom: 1px solid black;">
            <span style="cursor: pointer; cursor: hand;" onclick="closeColorKey(event);">
              <xsl:value-of select="resources:GetString('Preview_ColorKey_Title_Name')"/> | X
            </span>
          </div>
          <table>
            <tr>
              <td class="fuzzy">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_Fuzzy')"/>
              </td>
            </tr>
            <tr>
              <td class="interactive">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_Interactive')"/>
              </td>
            </tr>
            <tr>
              <td class="context-match">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_ContextMatch')"/>
              </td>
            </tr>
            <tr>
              <td class="mt">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_MT')"/>
              </td>
            </tr>
            <tr>
              <td class="tm">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_TM')"/>
              </td>
            </tr>
            <tr>
              <td class="not-translated">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_NotTranslated')"/>
              </td>
            </tr>
            <tr>
              <td class="auto-propagated">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_AutoPropagated')"/>
              </td>
            </tr>
            <tr>
              <td class="source">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_Source')"/>
              </td>
            </tr>
            <tr>
              <td class="Low">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_LowComment')"/>
              </td>
            </tr>
            <tr>
              <td class="Medium">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_MediumComment')"/>
              </td>
            </tr>
            <tr>
              <td class="High">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_HighComment')"/>
              </td>
            </tr>
            <tr>
              <td class="locked">
                <xsl:value-of select="resources:GetString('Preview_ColorKey_Locked')"/>
              </td>
            </tr>
          </table>
        </div>
        </div>
      </body>
		</html>
	</xsl:template>

	<xsl:template match="file">
    <!-- Inserted BR so that options box doesn't cover top of document -->
    <br/>
		<table id="file" cellspacing="0" cellpadding="2">
			<xsl:apply-templates select="paragraph"/>
		</table>
	</xsl:template>

	<xsl:template match="source"><xsl:variable name="rightAlign"><xsl:if test="@rightAlign = 'True'"><xsl:text>rightAlign</xsl:text></xsl:if></xsl:variable><td class="sourceCell {$rightAlign}"><xsl:apply-templates/></td></xsl:template>
	<xsl:template match="target"><xsl:variable name="rightAlign"><xsl:if test="@rightAlign = 'True'"><xsl:text>rightAlign</xsl:text></xsl:if></xsl:variable><td class="targetCell {$rightAlign}"><xsl:apply-templates/></td></xsl:template>
  <xsl:template match="context"><xsl:variable name="contextColor"><xsl:if test="@contextColor"><xsl:value-of select="@contextColor"/></xsl:if></xsl:variable><td class="contextCell" style="background-color: {$contextColor};" width="10%"><xsl:apply-templates/></td></xsl:template>
	<xsl:template match="paragraph"><tr class="filetable"><xsl:apply-templates select="source"/><xsl:apply-templates select="target"/><xsl:apply-templates select="context"/></tr></xsl:template>
	<xsl:template xml:space="preserve" match="segment"><xsl:choose><xsl:when test="@translationOrigin"><span class="hideTransOrigin {@translationOrigin}"><xsl:apply-templates/>&#160;</span></xsl:when><xsl:otherwise><span><xsl:apply-templates/>&#160;</span></xsl:otherwise></xsl:choose></xsl:template>
  <xsl:template xml:space="preserve" match="tagpair"><span class="tag"><xsl:value-of select="@name"/></span><xsl:apply-templates/><span class="tag"><xsl:value-of select="@name"/></span></xsl:template>
  <xsl:template match="placeholder"><span class="tag"><xsl:choose><xsl:when test="@textEquiv"><xsl:value-of select="@textEquiv"/></xsl:when><xsl:otherwise><xsl:value-of select="@name"/></xsl:otherwise></xsl:choose></span></xsl:template>
  <xsl:template xml:space="preserve" match="locked"><span class="locked"><xsl:apply-templates/></span></xsl:template>
  <xsl:template xml:space="preserve" match="comment"><span class="{@severity} commentBlock" onclick="changeCommentDisplay('{@commentId}');"><xsl:apply-templates/></span><span id="{@commentId}" class="commentText hideComment">&#160;(<xsl:value-of select="@text"/>)&#160;</span></xsl:template>
  <xsl:template xml:space="preserve" match="formatting"><xsl:variable name="font-family"><xsl:if test="@font-family"><xsl:text>font-family: </xsl:text><xsl:value-of select="@font-family"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="font-size"><xsl:if test="@font-size"><xsl:text>font-size: </xsl:text><xsl:value-of select="@font-size"/><xsl:text>pt;</xsl:text></xsl:if></xsl:variable><xsl:variable name="font-weight"><xsl:if test="@font-weight"><xsl:text>font-weight: </xsl:text><xsl:value-of select="@font-weight"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="font-style"><xsl:if test="@font-style"><xsl:text>font-style: </xsl:text><xsl:value-of select="@font-style"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="text-decoration"><xsl:if test="@text-decoration"><xsl:text>text-decoration: </xsl:text><xsl:value-of select="@text-decoration"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="text-color"><xsl:if test="@text-color"><xsl:text>color: </xsl:text><xsl:value-of select="@text-color"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="background-color"><xsl:if test="@background-color"><xsl:text>background-color: </xsl:text><xsl:value-of select="@background-color"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><xsl:variable name="vertical-align"><xsl:if test="@vertical-align"><xsl:text>vertical-align: </xsl:text><xsl:value-of select="@vertical-align"/><xsl:text>;</xsl:text></xsl:if></xsl:variable><span style="{$font-family}{$font-size}{$font-weight}{$font-style}{$text-decoration}{$text-color}{$background-color}{$vertical-align}"><xsl:apply-templates/></span></xsl:template>
	<xsl:template xml:space="preserve" match="external"><span class="external"><xsl:apply-templates/></span></xsl:template>
  <xsl:template xml:space="preserve" match="text"><xsl:choose><xsl:when test="string(.) = ''"><span><xsl:text>&#160;</xsl:text></span></xsl:when><xsl:otherwise><span><xsl:value-of select="." /></span></xsl:otherwise></xsl:choose></xsl:template>
  <xsl:template xml:space="preserve" match="revision"><xsl:choose><xsl:when test="@deleted"><span class="deleted"><xsl:apply-templates/></span></xsl:when><xsl:otherwise><span class="inserted"><xsl:apply-templates/></span></xsl:otherwise></xsl:choose></xsl:template>
</xsl:stylesheet>