import ModelClient, { isUnexpected } from '@azure-rest/ai-inference';
import { AzureKeyCredential } from '@azure/core-auth';
import { env } from 'cloudflare:workers';
import { AutoRouter, cors  } from 'itty-router'; // ~1kB

const { preflight, corsify } = cors()

const router = AutoRouter({
	before: [preflight],  // add preflight upstream
	finally: [corsify],   // and corsify downstream
});
const GITHUB_TOKEN = env.GITHUB_TOKEN;

const foods = ['Boiled Potatoes with Dill', 'Potato Salad', 'Beetroot Salad'];

interface Query {
	finishedCount: number;
	ruinedCount: number;
	// left ingredients
	Potatoes: number;
	Salt: number;
	Dill: number;
	Mayonnaise: number;
	Pickles: number;
	Beets: number;
}

const systemContent = `You are a sarcastic, rude, and mean restaurant manager in a cooking-themed game.
Always give a score from 0 to 10.
Then provide one short, harsh sentence (maximum 10 words) judging the cook’s performance.
Do not be nice. Do not summarize. Do not praise.
Always respond in a single line with no line breaks.
Never accept a situation where 0 dish is finished, give 0/10.
Maintain a critical tone with biting wit.
Examples of acceptable responses:
"Good job, but you can do better."
"You did well, but there's room for improvement."
"Don't be a cook."
`;

const userContentCreater = (name: string, query: Query) => {
	let left = '';
	if (query.Potatoes > 0) {
		left += `${query.Potatoes} Potatoes, `;
	}
	if (query.Salt > 0) {
		left += `${query.Salt} Salt, `;
	}
	if (query.Dill > 0) {
		left += `${query.Dill} Dill, `;
	}
	if (query.Mayonnaise > 0) {
		left += `${query.Mayonnaise} Mayonnaise, `;
	}
	if (query.Pickles > 0) {
		left += `${query.Pickles} Pickles, `;
	}
	if (query.Beets > 0) {
		left += `${query.Beets} Beets, `;
	}
	if (left.length > 0) {
		left = left.slice(0, -2);
	} else {
		left = 'nothing';
	}

	return `I cooked ${name}.I finished ${query.finishedCount} dishes, ruined ${query.ruinedCount} dish, and I have ${left} left.`;
};

router.get('/review/:name', async ({ name, query }) => {
	name = name.replace(/_/g, ' ');
	if (!foods.includes(name)) {
		return new Response('Food not found', { status: 404 });
	}
	const client = ModelClient('https://models.github.ai/inference', new AzureKeyCredential(GITHUB_TOKEN));

	const response = await client.path('/chat/completions').post({
		body: {
			messages: [
				{ role: 'system', content: systemContent },
				{ role: 'user', content: userContentCreater(name, query as unknown as Query) },
			],
			model: 'openai/gpt-4.1-nano',
			temperature: 1.5,
			max_tokens: 24,
			top_p: 0.5,
		},
	});

	if (isUnexpected(response)) {
		throw response.body.error;
	}

	return response.body.choices[0].message.content;
});

export default router;
