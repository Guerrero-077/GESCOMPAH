import {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatOptgroup,
  MatOption,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger
} from "./chunk-KS7JTUXC.js";
import "./chunk-HJMD3B2T.js";
import "./chunk-JKRSZR5T.js";
import {
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatPrefix,
  MatSuffix
} from "./chunk-3WS6ADPA.js";
import "./chunk-2DHZ7P4C.js";
import "./chunk-SQU5GB3G.js";
import "./chunk-4WO7TCKM.js";
import "./chunk-IXSWVYJF.js";
import "./chunk-IYFSTEKZ.js";
import "./chunk-IQDINQFA.js";
import "./chunk-LZW6PRXT.js";
import "./chunk-QCETVJKM.js";
import "./chunk-KAPXTIMC.js";
import "./chunk-GVTDVR5J.js";
import "./chunk-5ZSLPG3I.js";
import "./chunk-V7F7GZAJ.js";
import "./chunk-EOFW2REK.js";
import "./chunk-5LYE4BBP.js";
import "./chunk-VI6FU7OM.js";
import "./chunk-CT6G2XIJ.js";
import "./chunk-P3FDRWUN.js";
import "./chunk-6SJCIUR7.js";
import "./chunk-OBKGRE2Y.js";
import "./chunk-7DPHYZ4E.js";
import "./chunk-QYCCAYNY.js";
import "./chunk-3KKC7HMJ.js";
import "./chunk-XWLXMCJQ.js";

// node_modules/@angular/material/fesm2022/select.mjs
var matSelectAnimations = {
  // Represents
  // trigger('transformPanel', [
  //   state(
  //     'void',
  //     style({
  //       opacity: 0,
  //       transform: 'scale(1, 0.8)',
  //     }),
  //   ),
  //   transition(
  //     'void => showing',
  //     animate(
  //       '120ms cubic-bezier(0, 0, 0.2, 1)',
  //       style({
  //         opacity: 1,
  //         transform: 'scale(1, 1)',
  //       }),
  //     ),
  //   ),
  //   transition('* => void', animate('100ms linear', style({opacity: 0}))),
  // ])
  /** This animation transforms the select's overlay panel on and off the page. */
  transformPanel: {
    type: 7,
    name: "transformPanel",
    definitions: [
      {
        type: 0,
        name: "void",
        styles: {
          type: 6,
          styles: { opacity: 0, transform: "scale(1, 0.8)" },
          offset: null
        }
      },
      {
        type: 1,
        expr: "void => showing",
        animation: {
          type: 4,
          styles: {
            type: 6,
            styles: { opacity: 1, transform: "scale(1, 1)" },
            offset: null
          },
          timings: "120ms cubic-bezier(0, 0, 0.2, 1)"
        },
        options: null
      },
      {
        type: 1,
        expr: "* => void",
        animation: {
          type: 4,
          styles: { type: 6, styles: { opacity: 0 }, offset: null },
          timings: "100ms linear"
        },
        options: null
      }
    ],
    options: {}
  }
};
export {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatOptgroup,
  MatOption,
  MatPrefix,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger,
  MatSuffix,
  matSelectAnimations
};
//# sourceMappingURL=@angular_material_select.js.map
