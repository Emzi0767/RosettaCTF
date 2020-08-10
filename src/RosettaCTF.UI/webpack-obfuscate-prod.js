var WebpackObfuscator = require('webpack-obfuscator');

module.exports = {
    plugins: [
        new WebpackObfuscator ({
            stringArray: true,
            rotateStringArray: true,
            transformObjectKeys: false,
            unicodeEscapeSequence: true,
            numbersToExpressions: false,
            disableConsoleOutput: true,
            splitStrings: true
        }, [])
    ]
};
