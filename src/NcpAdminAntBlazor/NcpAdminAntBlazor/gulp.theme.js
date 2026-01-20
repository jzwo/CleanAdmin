let gulp = require('gulp'),
    cleanCss = require('gulp-clean-css'),
    less = require('gulp-less'),
    rename = require('gulp-rename'),
    npmImport = require("less-plugin-npm-import"),
    footer = require('gulp-footer'),   // 用于在文件末尾追加内容
    plumber = require('gulp-plumber'); // 防止报错导致 Gulp 停止

// 定义要追加的内容
const injectContent = `
@import "./custom-vars.less";
@import "./export-variables.less";
`;

gulp.task('styles', function () {
    return gulp.src([
        'styles/**/*.less',
        '!styles/components.less',
        '!styles/custom-vars.less',
        '!styles/export-variables.less',
    ])
        .pipe(plumber()) // 错误处理：编译出错不会让 gulp 挂掉
        .pipe(footer(injectContent)) // 在末尾追加 import
        .pipe(less({
            javascriptEnabled: true,
            plugins: [new npmImport({prefix: '~'})]
        }))
        .pipe(cleanCss({compatibility: '*'}))
        .pipe(rename({dirname: 'wwwroot/styles'}))
        .pipe(gulp.dest('./'));
});