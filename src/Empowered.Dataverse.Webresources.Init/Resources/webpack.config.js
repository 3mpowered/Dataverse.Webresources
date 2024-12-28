import {resolve, join, basename, relative, dirname} from "path";
import {globbySync} from "globby";
import {TsconfigPathsPlugin} from "tsconfig-paths-webpack-plugin";

const ROOT_DIR = resolve();
const OUTPUT_DIR = join(ROOT_DIR, "./dist");
const entryPoints = globbySync(["src/form/**/*.form.ts", "src/command/**/*.command.ts", "src/view/**/*.view.ts"], {
    absolute: true,
}).reduce((entries, file) => {
    const entryPoint = join("scripts", relative("src", dirname(file)), basename(file, ".ts"));
    entries[entryPoint] = file;
    return entries;
}, {});

const configuration = {
    entry: entryPoints,
    output: {
        path: OUTPUT_DIR,
        filename: "[name].js",
        library: "<globalNamespace>",
        libraryTarget: "assign-properties",
        clean: true,
    },
    resolve: {
        extensions: [".js", ".ts"],
        plugins: [new TsconfigPathsPlugin()],
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /node_modules/,
                use: ["babel-loader", "ts-loader"],
            },
        ],
    },
    plugins: [],
};

export default configuration;
