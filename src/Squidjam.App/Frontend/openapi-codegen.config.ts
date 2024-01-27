import { generateSchemaTypes, generateReactQueryComponents } from '@openapi-codegen/typescript';
import { defineConfig } from '@openapi-codegen/cli';
export default defineConfig({
    squidjam: {
        from: {
            source: 'url',
            url: 'http://localhost:5038/swagger/v1/swagger.json',
        },
        outputDir: './src/queries/api',
        to: async (context) => {
            const filenamePrefix = 'squidjam';
            const { schemasFiles } = await generateSchemaTypes(context, {
                filenamePrefix,
            });
            await generateReactQueryComponents(context, {
                filenamePrefix,
                schemasFiles,
            });
        },
    },
});
