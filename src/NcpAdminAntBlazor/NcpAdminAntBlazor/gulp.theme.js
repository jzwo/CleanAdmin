var gulp = require('gulp'),
    cleanCss = require('gulp-clean-css'),
    less = require('gulp-less'),
    rename = require('gulp-rename'),
    concatCss = require("gulp-concat-css"),
    npmImport = require("less-plugin-npm-import"),
    through = require('through2');

// 自动注入 export-variables.less 的插件
function injectExportVariables() {
    const exportVarsImport = "@import './export-variables.less';\n";

    return through.obj(function (file, enc, cb) {
        if (file.isBuffer()) {
            const content = file.contents.toString();
            // 检查是否已经包含 export-variables.less 的导入
            if (!content.includes("export-variables.less")) {
                // 在文件末尾添加导入语句
                file.contents = Buffer.from(content + '\n' + exportVarsImport);
            }
        }
        cb(null, file);
    });
}

gulp.task('themes', function () {
    return gulp.src([
        'themes/**/*.less',
        '!themes/components.less',
        '!themes/custom-vars.less',
        '!themes/export-variables.less',
    ])
        .pipe(injectExportVariables())
        .pipe(less({
            javascriptEnabled: true,
            plugins: [new npmImport({prefix: '~'})]
        }))
        .pipe(cleanCss({compatibility: '*'}))
        .pipe(rename({dirname: 'wwwroot/theme'}))
        .pipe(gulp.dest('./'));
});