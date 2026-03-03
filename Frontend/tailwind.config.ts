import type { Config } from 'tailwindcss';
// Optional: import defaultTheme if you want to include default fallback fonts
// const defaultTheme = require('tailwindcss/defaultTheme'); 

const config: Config = {
    content: [
        './pages/**/*.{js,ts,jsx,tsx,mdx}',
        './components/**/*.{js,ts,jsx,tsx,mdx}',
        './app/**/*.{js,ts,jsx,tsx,mdx}',
    ],
    theme: {
        extend: {
            fontFamily: {
                // Add 'montserrat' as a custom font family name
                montserrat: ['var(--font-montserrat)', 'sans-serif'],
                // Example with default fallback:
                // sans: ['var(--font-montserrat)', ...defaultTheme.fontFamily.sans],
            },
        },
    },
    plugins: [],
};

export default config;
