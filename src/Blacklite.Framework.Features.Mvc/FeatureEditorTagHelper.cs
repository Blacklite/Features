﻿using Blacklite.Framework.Features.Editors;
using Blacklite.Framework.Features.Editors.JsonEditors.Resolvers;
using Blacklite.Json.Schema;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features.Mvc
{
    [TargetElement("feature-editor")]
    public class FeatureEditorTagHelper : TagHelper
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly TabsJsonEditorResolver _tabsJsonEditorResolver;

        public FeatureEditorTagHelper(IJsonEditorProvider editorProvider, TabsJsonEditorResolver tabsJsonEditorResolver)
        {
            _editorProvider = editorProvider;
            _tabsJsonEditorResolver = tabsJsonEditorResolver;
        }

        [Required]
        public IFeatureEditor Editor { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var renderer = _editorProvider.GetJsonEditor(Editor.Schema, Editor.Prefix, _tabsJsonEditorResolver).Build();
            output.Content.Append(renderer.Render(Editor.Model));
        }
    }

    [TargetElement("feature-editor-script")]
    public class FeatureEditorScriptTagHelper : TagHelper
    {
        private readonly IJsonEditorProvider _editorProvider;
        private readonly TabsJsonEditorResolver _tabsJsonEditorResolver;

        public FeatureEditorScriptTagHelper(IJsonEditorProvider editorProvider, TabsJsonEditorResolver tabsJsonEditorResolver)
        {
            _editorProvider = editorProvider;
            _tabsJsonEditorResolver = tabsJsonEditorResolver;
        }

        [Required]
        public IFeatureEditor Editor { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            var renderer = _editorProvider.GetJsonEditor(Editor.Schema, Editor.Prefix, _tabsJsonEditorResolver).Build();
            var jsContent = renderer.JavaScript(Editor.Model);
            output.Content.Append($@"
$(function() {{

    var size = 42;
    $.fn.bootstrapSwitch.defaults.size = 'mini';
    $.fn.bootstrapSwitch.defaults.labelWidth = size / 2;
    $.fn.bootstrapSwitch.defaults.handleWidth = size;

    // bug in bootswitch :(
    $.fn.bootstrapSwitch.Constructor.prototype._width = function () {{
        var $handles, handleWidth;
        $handles = this.$on.add(this.$off);
        $handles.add(this.$label).css('width', '');
        var handleAuto = this.options.handleWidth === 'auto';
        var labelAuto = this.options.labelWidth === 'auto';
        handleWidth = handleAuto ? Math.max(this.$on.width(), this.$off.width()) : this.options.handleWidth;
        $handles.css('width', handleWidth);
        this.$label.css('width', (function (_this) {{
            return function (index, width) {{
                if (_this.options.labelWidth !== 'auto') {{
                    return _this.options.labelWidth;
                }}
                if (width < handleWidth) {{
                        return handleWidth;
                }} else {{
                    return width;
                }}
            }};
        }})(this));
        this._handleWidth = handleAuto ? this.$on.outerWidth() : this.options.handleWidth;
        this._labelWidth = labelAuto ? this.$label.outerWidth() : this.options.labelWidth;
        this.$container.css('width', (this._handleWidth * 2) + this._labelWidth);
        return this.$wrapper.css('width', this._handleWidth + this._labelWidth);
    }}

    $('input[type=\'checkbox\']').bootstrapSwitch();
    $('[data-toggle=""popover""]').popover();

    var elements = {{}};
    var callbacks = [];

    {jsContent}

    for (var i in elements) {{
        if (i.indexOf('_') > -1) {{
        }}
    }}
    callbacks.forEach(function(x) {{ x(); }});
}});");
        }
    }
}
