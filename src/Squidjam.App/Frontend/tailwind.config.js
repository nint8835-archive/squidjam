/** @type {import('tailwindcss').Config} */
export default {
    content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
    theme: {
        extend: {
            fontFamily: {
                gump: ['Lyhyt Lauantai'],
            },
        },
        heroPatterns: {
            eyes: require('tailwindcss-hero-patterns/src/patterns').eyes,
            floatingcogs: require('tailwindcss-hero-patterns/src/patterns').floatingcogs,
        },
        heroPatternsColors: ['zinc'],
    },
    plugins: [require('tailwindcss-hero-patterns')],
};
