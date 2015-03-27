using Blacklite.Framework.Features.EditorModel;
using Blacklite.Framework.Features.EditorModel.JsonEditors.Resolvers;
using Blacklite.Json.Schema;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Blacklite.Framework.Features.Mvc
{
    public class FeatureEditorTagHelper : TagHelper
    {
        [Required]
        public IFeatureEditor Editor { get; set; }

        [Activate]
        public IJsonEditorProvider EditorProvider { get; private set; }

        [Activate]
        public TabsJsonEditorResolver TabsJsonEditorResolver { get; private set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var renderer = EditorProvider.GetJsonEditor(Editor.Schema, Editor.Prefix, TabsJsonEditorResolver).Build();
            output.Content = renderer.Render(Editor.Model);
            var jsContent = renderer.JavaScript(Editor.Model);
            /*

        elements['{item.id}'].data('bootstrapSwitch').onSwitchChange(function() {{
            callbacks.forEach(function(x) {{ x(); }});
        }}),*/
            output.Content += $@"<script>
window.document.ready = function() {{

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

    { jsContent}

    for (var i in elements) {{
        if (i.indexOf('_') > -1) {{
        }}
    }}
    callbacks.forEach(function(x) {{ x(); }});
}};
</script>";
        }
    }
}
